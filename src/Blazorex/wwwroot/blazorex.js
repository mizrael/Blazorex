const DEFAULT_CANVAS_OPTIONS = {
    alpha: false,
    desynchronized: true,
    colorSpace: "srgb",
    willReadFrequently: false
};
const MARSHAL_REFERENCE_PROPERTIES = ['fillStyle', 'strokeStyle'];
const PRINTABLE_KEYS = ['Enter', 'Tab', 'Backspace', 'Delete'];
export const createBlazorexAPI = () => {
    const canvasContexts = new Map();
    const elementCache = new Map();
    const marshalledObjects = new Map();
    const imageDataCache = new Map();
    const pressedKeys = new Set();
    const modifierKeys = {
        shift: false,
        ctrl: false,
        alt: false,
        meta: false
    };
    let nextImageId = 0;
    let cachedContexts = [];
    let keyboardEventContexts = [];
    let mouseEventContexts = [];
    let resizeEventContexts = [];
    const eventDataExtractors = {
        wheel: (event) => ({
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            clientX: event.clientX,
            clientY: event.clientY
        }),
        mousedown: (event) => ({
            clientX: event.clientX,
            clientY: event.clientY,
            button: event.button
        }),
        mouseup: (event) => ({
            clientX: event.clientX,
            clientY: event.clientY,
            button: event.button
        }),
        touchstart: (event) => ({
            clientX: event.touches[0]?.clientX ?? 0,
            clientY: event.touches[0]?.clientY ?? 0,
            button: 0
        }),
        touchend: (event) => ({
            clientX: event.changedTouches[0]?.clientX ?? 0,
            clientY: event.changedTouches[0]?.clientY ?? 0,
            button: 0
        })
    };
    const blazorMethodNames = {
        wheel: 'Wheel',
        mousedown: 'MouseDown',
        mouseup: 'MouseUp',
        touchstart: 'MouseDown',
        touchend: 'MouseUp'
    };
    const isMarshalReference = (param) => {
        return typeof param === 'object' &&
            param !== null &&
            'id' in param &&
            'isElementRef' in param &&
            typeof param.id === 'number';
    };
    const createCanvasEventHandler = (blazorInstance, eventType) => {
        const methodName = blazorMethodNames[eventType];
        return (domEvent) => {
            let eventData;
            switch (eventType) {
                case 'wheel':
                    eventData = eventDataExtractors.wheel(domEvent);
                    break;
                case 'mousedown':
                    eventData = eventDataExtractors.mousedown(domEvent);
                    break;
                case 'mouseup':
                    eventData = eventDataExtractors.mouseup(domEvent);
                    break;
                case 'touchstart':
                    eventData = eventDataExtractors.touchstart(domEvent);
                    break;
                case 'touchend':
                    eventData = eventDataExtractors.touchend(domEvent);
                    break;
                default:
                    return;
            }
            blazorInstance
                .invokeMethodAsync(methodName, eventData)
                .catch(error => console.error(`Failed to invoke ${methodName}:`, error));
        };
    };
    const updateModifierKeyState = (event) => {
        modifierKeys.shift = event.shiftKey;
        modifierKeys.ctrl = event.ctrlKey;
        modifierKeys.alt = event.altKey;
        modifierKeys.meta = event.metaKey;
    };
    const getCachedElement = (elementRef) => {
        const cacheKey = `_bl_${elementRef.id}`;
        let element = elementCache.get(cacheKey);
        if (!element) {
            element = document.querySelector(`[${cacheKey}]`) ?? undefined;
            if (element) {
                elementCache.set(cacheKey, element);
            }
        }
        return element;
    };
    const unwrapPropertyValue = (value) => {
        if (value == null)
            return value;
        if (typeof value === 'string' || typeof value === 'number') {
            return value;
        }
        if (value instanceof Date) {
            return value;
        }
        if (typeof value === 'object') {
            const wrapper = value;
            if ('value' in wrapper) {
                return wrapper.value;
            }
            if ('id' in wrapper) {
                return wrapper.id;
            }
            if ('result' in wrapper) {
                return wrapper.result;
            }
            return value;
        }
        return value;
    };
    const refreshContextCaches = () => {
        const allContexts = Array.from(canvasContexts.values());
        keyboardEventContexts = allContexts;
        mouseEventContexts = allContexts;
        resizeEventContexts = allContexts;
    };
    const safeInvokeBlazorMethod = async (instance, methodName, ...args) => {
        try {
            await instance.invokeMethodAsync(methodName, ...args);
        }
        catch (error) {
            console.error(`Failed to invoke Blazor method '${methodName}':`, error);
        }
    };
    const invokeContextMethod = (context, methodName, parameters) => {
        const params = parameters ? [...parameters] : [];
        const typedContext = context;
        if (params.length === 0) {
            return typedContext[methodName]();
        }
        const firstParam = params[0];
        if (isMarshalReference(firstParam)) {
            const marshalRef = firstParam;
            if (!marshalRef.isElementRef) {
                params.splice(0, 1);
                const existingObject = marshalledObjects.get(marshalRef.id);
                if (existingObject?.[methodName]) {
                    if (marshalRef.classInitializer) {
                        const Constructor = globalThis[marshalRef.classInitializer];
                        existingObject[methodName](new Constructor(...params));
                    }
                    else {
                        existingObject[methodName](...params);
                    }
                    return;
                }
                const result = typedContext[methodName](...params);
                marshalledObjects.set(marshalRef.id, result);
                return marshalRef.id;
            }
            params[0] = getCachedElement(marshalRef);
        }
        const result = typedContext[methodName](...params);
        if (isMarshalReference(firstParam) && !firstParam.isElementRef) {
            marshalledObjects.set(firstParam.id, result);
        }
        return result;
    };
    const setContextProperty = (context, propertyName, value) => {
        const unwrappedValue = unwrapPropertyValue(value);
        const typedContext = context;
        if (MARSHAL_REFERENCE_PROPERTIES.includes(propertyName)) {
            if (typeof unwrappedValue === 'number') {
                const resolvedValue = marshalledObjects.get(unwrappedValue) ?? unwrappedValue;
                typedContext[propertyName] = resolvedValue;
            }
            else {
                typedContext[propertyName] = unwrappedValue;
            }
        }
        else {
            typedContext[propertyName] = unwrappedValue;
        }
    };
    const mouseCoordinateBuffer = {
        clientX: 0,
        clientY: 0,
        offsetX: 0,
        offsetY: 0
    };
    const handleGlobalKeyUp = (event) => {
        const { keyCode, key } = event;
        updateModifierKeyState(event);
        if (keyboardEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        pressedKeys.delete(keyCode);
        const keyEventData = {
            keyCode,
            key,
            isHeld: false,
            heldKeys: Array.from(pressedKeys),
            modifiers: { ...modifierKeys }
        };
        keyboardEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'KeyReleased', keyEventData);
        });
    };
    const handleGlobalKeyDown = (event) => {
        const { keyCode, key } = event;
        updateModifierKeyState(event);
        if (keyboardEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        pressedKeys.add(keyCode);
        const keyEventData = {
            keyCode,
            key,
            isHeld: true,
            heldKeys: Array.from(pressedKeys),
            modifiers: { ...modifierKeys }
        };
        keyboardEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'KeyPressed', keyEventData);
        });
        if (key.length === 1 || PRINTABLE_KEYS.includes(key)) {
            keyboardEventContexts.forEach(({ managedInstance }) => {
                safeInvokeBlazorMethod(managedInstance, 'KeyPressed', keyEventData);
            });
        }
    };
    const handleGlobalMouseMove = (event) => {
        if (mouseEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        mouseCoordinateBuffer.clientX = event.clientX;
        mouseCoordinateBuffer.clientY = event.clientY;
        mouseCoordinateBuffer.offsetX = event.offsetX;
        mouseCoordinateBuffer.offsetY = event.offsetY;
        mouseEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'MouseMoved', mouseCoordinateBuffer);
        });
    };
    const handleGlobalTouchMove = (event) => {
        if (mouseEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        const touch = event.touches[0];
        if (touch) {
            mouseCoordinateBuffer.clientX = touch.clientX;
            mouseCoordinateBuffer.clientY = touch.clientY;
            mouseCoordinateBuffer.offsetX = touch.clientX;
            mouseCoordinateBuffer.offsetY = touch.clientY;
            mouseEventContexts.forEach(({ managedInstance }) => {
                safeInvokeBlazorMethod(managedInstance, 'MouseMoved', mouseCoordinateBuffer);
            });
        }
    };
    const handleGlobalResize = () => {
        if (resizeEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        const { innerWidth, innerHeight } = window;
        resizeEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'Resized', innerWidth, innerHeight);
        });
    };
    const initCanvas = (canvasId, blazorInstance, contextOptions = DEFAULT_CANVAS_OPTIONS) => {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn(`Canvas element with ID '${canvasId}' not found`);
            return;
        }
        const eventHandlers = {
            wheel: createCanvasEventHandler(blazorInstance, 'wheel'),
            mousedown: createCanvasEventHandler(blazorInstance, 'mousedown'),
            mouseup: createCanvasEventHandler(blazorInstance, 'mouseup'),
            touchstart: createCanvasEventHandler(blazorInstance, 'touchstart'),
            touchend: createCanvasEventHandler(blazorInstance, 'touchend')
        };
        const eventOptions = { passive: false };
        Object.entries(eventHandlers).forEach(([eventType, handler]) => {
            canvas.addEventListener(eventType, handler, eventOptions);
        });
        const renderingContext = canvas.getContext('2d', contextOptions);
        if (!renderingContext) {
            console.error(`Failed to get 2D rendering context for canvas '${canvasId}'`);
            return;
        }
        canvasContexts.set(canvasId, {
            id: canvasId,
            context: renderingContext,
            managedInstance: blazorInstance,
            contextOptions
        });
        requestAnimationFrame(onFrameUpdate);
    };
    const onFrameUpdate = (timestamp) => {
        if (cachedContexts.length !== canvasContexts.size) {
            cachedContexts = Array.from(canvasContexts.values());
        }
        cachedContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'UpdateFrame', timestamp);
        });
        requestAnimationFrame(onFrameUpdate);
    };
    const createImageData = (canvasId, width, height) => {
        const contextInfo = canvasContexts.get(canvasId);
        if (!contextInfo) {
            console.warn(`Canvas context '${canvasId}' not found`);
            return null;
        }
        const imageData = contextInfo.context.createImageData(width, height);
        const imageId = nextImageId++;
        imageDataCache.set(imageId, imageData);
        return imageId;
    };
    const putImageData = (canvasId, imageId, pixelData, x, y) => {
        const contextInfo = canvasContexts.get(canvasId);
        const imageData = imageDataCache.get(imageId);
        if (!contextInfo || !imageData) {
            console.warn(`Invalid canvas '${canvasId}' or image '${imageId}'`);
            return;
        }
        imageData.data.set(pixelData);
        contextInfo.context.putImageData(imageData, x, y);
    };
    const processBatch = (canvasId, operations) => {
        if (!operations?.length)
            return;
        const contextInfo = canvasContexts.get(canvasId);
        if (!contextInfo) {
            console.warn(`Canvas context '${canvasId}' not found`);
            return;
        }
        const { context } = contextInfo;
        operations.forEach(({ methodName, args, isProperty }) => {
            if (isProperty) {
                setContextProperty(context, methodName, args);
            }
            else {
                invokeContextMethod(context, methodName, args);
            }
        });
    };
    const directCall = (canvasId, methodName, parameters) => {
        const contextInfo = canvasContexts.get(canvasId);
        if (!contextInfo) {
            console.warn(`Canvas context '${canvasId}' not found`);
            return null;
        }
        return invokeContextMethod(contextInfo.context, methodName, parameters);
    };
    const removeContext = (canvasId) => {
        return canvasContexts.delete(canvasId);
    };
    const resizeCanvas = (canvasId, width, height) => {
        const contextInfo = canvasContexts.get(canvasId);
        const canvas = document.getElementById(canvasId);
        if (!contextInfo || !canvas) {
            console.warn(`Canvas '${canvasId}' not found for resize operation`);
            return;
        }
        canvas.width = width;
        canvas.height = height;
        const newContext = canvas.getContext('2d', contextInfo.contextOptions);
        if (newContext) {
            contextInfo.context = newContext;
            safeInvokeBlazorMethod(contextInfo.managedInstance, 'Resized', width, height);
        }
    };
    const globalEventOptions = { passive: true };
    addEventListener('keyup', handleGlobalKeyUp, globalEventOptions);
    addEventListener('keydown', handleGlobalKeyDown, globalEventOptions);
    addEventListener('mousemove', handleGlobalMouseMove, globalEventOptions);
    addEventListener('touchmove', handleGlobalTouchMove, globalEventOptions);
    addEventListener('resize', handleGlobalResize, globalEventOptions);
    return Object.freeze({
        initCanvas,
        onFrameUpdate,
        createImageData,
        putImageData,
        processBatch,
        directCall,
        removeContext,
        resizeCanvas
    });
};
export default createBlazorexAPI;
