import type {
    BlazorexAPI,
    CanvasContextOptions,
    ContextInfo,
    DotNetObjectReference,
    MarshalReference,
    PropertyValue,
    PropertyValueWrapper,
    EventType,
    EventHandler,
    EventCoordinates,
    WheelEventData,
    KeyEventData,
    MouseEventCoordinates,
    ModifierState,
    BatchOperation,
    EventExtractors,
    EventMethodMap,
    CreateBlazorexAPI
} from './blazorex.types.js';

const DEFAULT_CANVAS_OPTIONS: CanvasContextOptions = {
    alpha: false,
    desynchronized: true,
    colorSpace: "srgb",
    willReadFrequently: false
};

const MARSHAL_REFERENCE_PROPERTIES = ['fillStyle', 'strokeStyle'] as const;
const PRINTABLE_KEYS = ['Enter', 'Tab', 'Backspace', 'Delete'];

/**
 * Creates a new Blazorex API instance with isolated state
 * @returns A frozen BlazorexAPI instance
 */
export const createBlazorexAPI: CreateBlazorexAPI = (): BlazorexAPI => {

    // ========================================
    // PRIVATE STATE MANAGEMENT
    // ========================================
    
    /** Active canvas contexts indexed by canvas ID */
    const canvasContexts = new Map<string, ContextInfo>();
    
    /** Cached DOM elements indexed by Blazor element reference ID */
    const elementCache = new Map<string, Element>();
    
    /** 
     * Marshal references for complex canvas objects (gradients, patterns, etc.)
     * Maps C# MarshalReference.Id to the actual JavaScript canvas object
     */
    const marshalledObjects = new Map<number, unknown>();
    
    /** ImageData objects indexed by auto-generated ID */
    const imageDataCache = new Map<number, ImageData>();
    
    /** Currently held keyboard keys */
    const pressedKeys = new Set<number>();
    
    /** Current state of modifier keys */
    const modifierKeys: ModifierState = { 
        shift: false, 
        ctrl: false, 
        alt: false, 
        meta: false 
    };
    
    /** Auto-incrementing counter for ImageData IDs */
    let nextImageId = 0;
    
    // Performance optimization: Pre-allocated context arrays
    let cachedContexts: ContextInfo[] = [];
    let keyboardEventContexts: ContextInfo[] = [];
    let mouseEventContexts: ContextInfo[] = [];
    let resizeEventContexts: ContextInfo[] = [];
    
    // ========================================
    // EVENT EXTRACTION & MAPPING
    // ========================================
    
    /** Extract relevant data from different event types */
    const eventDataExtractors: EventExtractors = {
        wheel: (event: WheelEvent): WheelEventData => ({
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            clientX: event.clientX,
            clientY: event.clientY
        }),
        
        mousedown: (event: MouseEvent): EventCoordinates => ({
            clientX: event.clientX,
            clientY: event.clientY,
            button: event.button
        }),
        
        mouseup: (event: MouseEvent): EventCoordinates => ({
            clientX: event.clientX,
            clientY: event.clientY,
            button: event.button
        }),
        
        touchstart: (event: TouchEvent): EventCoordinates => ({
            clientX: event.touches[0]?.clientX ?? 0,
            clientY: event.touches[0]?.clientY ?? 0,
            button: 0
        }),
        
        touchend: (event: TouchEvent): EventCoordinates => ({
            clientX: event.changedTouches[0]?.clientX ?? 0,
            clientY: event.changedTouches[0]?.clientY ?? 0,
            button: 0
        })
    } as const;
    
    /** Map event types to Blazor method names */
    const blazorMethodNames: EventMethodMap = {
        wheel: 'Wheel',
        mousedown: 'MouseDown',
        mouseup: 'MouseUp',
        touchstart: 'MouseDown', // Touch events map to mouse events
        touchend: 'MouseUp'
    } as const;
    
    // ========================================
    // UTILITY FUNCTIONS
    // ========================================
    
    /**
     * Determines if a parameter is a MarshalReference from C#
     */
    const isMarshalReference = (param: unknown): param is MarshalReference => {
        return typeof param === 'object' && 
               param !== null && 
               'id' in param && 
               'isElementRef' in param &&
               typeof (param as any).id === 'number';
    };
    
    /**
     * Creates a strongly-typed event handler for canvas events
     */
    const createCanvasEventHandler = (
        blazorInstance: DotNetObjectReference, 
        eventType: EventType
    ): EventHandler => {
        const methodName = blazorMethodNames[eventType];
        
        return (domEvent: Event): void => {
            let eventData: EventCoordinates | WheelEventData;
            
            switch (eventType) {
                case 'wheel':
                    eventData = eventDataExtractors.wheel(domEvent as WheelEvent);
                    break;
                case 'mousedown':
                    eventData = eventDataExtractors.mousedown(domEvent as MouseEvent);
                    break;
                case 'mouseup':
                    eventData = eventDataExtractors.mouseup(domEvent as MouseEvent);
                    break;
                case 'touchstart':
                    eventData = eventDataExtractors.touchstart(domEvent as TouchEvent);
                    break;
                case 'touchend':
                    eventData = eventDataExtractors.touchend(domEvent as TouchEvent);
                    break;
                default:
                    return;
            }
            
            blazorInstance
                .invokeMethodAsync(methodName, eventData)
                .catch(error => console.error(`Failed to invoke ${methodName}:`, error));
        };
    };
    
    /**
     * Updates the modifier key state from a keyboard event
     */
    const updateModifierKeyState = (event: KeyboardEvent): void => {
        modifierKeys.shift = event.shiftKey;
        modifierKeys.ctrl = event.ctrlKey;
        modifierKeys.alt = event.altKey;
        modifierKeys.meta = event.metaKey;
    };
    
    /**
     * Retrieves a cached DOM element by Blazor ElementRef
     */
    const getCachedElement = (elementRef: MarshalReference): Element | undefined => {
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
    
    /**
     * Unwraps property values that may come from Blazor in various formats
     * Handles: string, number, Date, objects with { value }, objects with { id }, objects with { result }
     */
    const unwrapPropertyValue = (value: PropertyValue): string | number | Date | unknown => {
        // Handle null/undefined
        if (value == null) return value;
        
        // Handle primitive types directly
        if (typeof value === 'string' || typeof value === 'number') {
            return value;
        }
        
        // Handle Date objects
        if (value instanceof Date) {
            return value;
        }
        
        // Handle object wrappers
        if (typeof value === 'object') {
            const wrapper = value as PropertyValueWrapper;
            
            // Check for value wrapper: { value: actualValue }
            if ('value' in wrapper) {
                return wrapper.value;
            }
            
            // Check for ID reference: { id: referenceId }
            if ('id' in wrapper) {
                return wrapper.id;
            }
            
            // Check for result wrapper: { result: actualValue }
            if ('result' in wrapper) {
                return wrapper.result;
            }
            
            // If it's an object but doesn't match our wrapper patterns, return as-is
            return value;
        }
        
        // Fallback for any other types
        return value;
    };
    
    /**
     * Updates all cached context arrays when contexts change
     */
    const refreshContextCaches = (): void => {
        const allContexts = Array.from(canvasContexts.values());
        keyboardEventContexts = allContexts;
        mouseEventContexts = allContexts;
        resizeEventContexts = allContexts;
    };
    
    /**
     * Safely invokes a Blazor method with error handling
     */
    const safeInvokeBlazorMethod = async (
        instance: DotNetObjectReference, 
        methodName: string, 
        ...args: unknown[]
    ): Promise<void> => {
        try {
            await instance.invokeMethodAsync(methodName, ...args);
        } catch (error) {
            console.error(`Failed to invoke Blazor method '${methodName}':`, error);
        }
    };
    
    // ========================================
    // CANVAS CONTEXT OPERATIONS
    // ========================================
    
    /** Canvas properties that can reference marshalled objects (gradients/patterns) */
    
    /**
     * Invokes a method on the canvas context with marshal reference support
     */
    const invokeContextMethod = (
        context: CanvasRenderingContext2D, 
        methodName: string, 
        parameters?: unknown[]
    ): unknown => {
        const params = parameters ? [...parameters] : [];
        const typedContext = context as unknown as Record<string, Function>;
        
        // Handle zero-parameter methods
        if (params.length === 0) {
            return typedContext[methodName]();
        }
        
        const firstParam = params[0];
        
        // Handle C# MarshalReference objects
        if (isMarshalReference(firstParam)) {

            const marshalRef = firstParam as MarshalReference;
            
            if (!marshalRef.isElementRef) {
                // This is a marshal object reference (gradient, pattern, etc.)
                params.splice(0, 1);
                
                const existingObject = marshalledObjects.get(marshalRef.id) as Record<string, Function> | undefined;
                if (existingObject?.[methodName]) {
                    if (marshalRef.classInitializer) {
                        const Constructor = (globalThis as any)[marshalRef.classInitializer];
                        existingObject[methodName](new Constructor(...params));
                    } else {
                        existingObject[methodName](...params);
                    }
                    return;
                }
                
                // Create new marshal object and store it
                const result = typedContext[methodName](...params);
                marshalledObjects.set(marshalRef.id, result);
                return marshalRef.id;
            }

            params[0] = getCachedElement(marshalRef);
        }
        
        
        const result = typedContext[methodName](...params);
        
        // If this created a marshal object, store it
        if (isMarshalReference(firstParam) && !firstParam.isElementRef) {
            marshalledObjects.set(firstParam.id, result);
        }
        
        return result;
    };
    
    /**
     * Sets a property on the canvas context with type unwrapping
     */
    const setContextProperty = (
        context: CanvasRenderingContext2D, 
        propertyName: string, 
        value: PropertyValue
    ): void => {
        const unwrappedValue = unwrapPropertyValue(value);
        const typedContext = context as unknown as Record<string, unknown>;
        
        // Special handling for properties that may reference gradients/patterns
        if (MARSHAL_REFERENCE_PROPERTIES.includes(propertyName as any)) {
            // Check if this is a marshal reference ID
            if (typeof unwrappedValue === 'number') {
                const resolvedValue = marshalledObjects.get(unwrappedValue) ?? unwrappedValue;
                typedContext[propertyName] = resolvedValue;
            } else {
                typedContext[propertyName] = unwrappedValue;
            }
        } else {
            typedContext[propertyName] = unwrappedValue;
        }
    };
    
    // ========================================
    // EVENT HANDLERS
    // ========================================
    
    /** Reusable mouse coordinate buffer to avoid allocations */
    const mouseCoordinateBuffer: MouseEventCoordinates = { 
        clientX: 0, 
        clientY: 0, 
        offsetX: 0, 
        offsetY: 0 
    };
    
    /**
     * Handles global keyboard key release events
     */
    const handleGlobalKeyUp = (event: KeyboardEvent): void => {
        const { keyCode, key } = event;
        updateModifierKeyState(event);
        
        if (keyboardEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        
        pressedKeys.delete(keyCode);
        
        const keyEventData: KeyEventData = {
            keyCode,
            key,
            isHeld: false,
            heldKeys: Array.from(pressedKeys),
            modifiers: { ...modifierKeys }
        };
        
        // Broadcast to all canvas contexts
        keyboardEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'KeyReleased', keyEventData);
        });
    };
    
    /**
     * Handles global keyboard key press events
     */
    const handleGlobalKeyDown = (event: KeyboardEvent): void => {
        const { keyCode, key } = event;
        updateModifierKeyState(event);
        
        if (keyboardEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        
        pressedKeys.add(keyCode);
        
        const keyEventData: KeyEventData = {
            keyCode,
            key,
            isHeld: true,
            heldKeys: Array.from(pressedKeys),
            modifiers: { ...modifierKeys }
        };
        
        // Broadcast key press to all contexts
        keyboardEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'KeyPressed', keyEventData);
        });
        
        // Send additional KeyPress event for printable characters
        if (key.length === 1 || PRINTABLE_KEYS.includes(key)) {
            keyboardEventContexts.forEach(({ managedInstance }) => {
                safeInvokeBlazorMethod(managedInstance, 'KeyPressed', keyEventData);
            });
        }
    };
    
    /**
     * Handles global mouse movement events
     */
    const handleGlobalMouseMove = (event: MouseEvent): void => {
        if (mouseEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        
        // Update reusable buffer to avoid object allocations
        mouseCoordinateBuffer.clientX = event.clientX;
        mouseCoordinateBuffer.clientY = event.clientY;
        mouseCoordinateBuffer.offsetX = event.offsetX;
        mouseCoordinateBuffer.offsetY = event.offsetY;
        
        mouseEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'MouseMoved', mouseCoordinateBuffer);
        });
    };
    
    /**
     * Handles global touch movement events (treats them as mouse moves)
     */
    const handleGlobalTouchMove = (event: TouchEvent): void => {
        if (mouseEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        
        // Use first touch point and treat as mouse move
        const touch = event.touches[0];
        if (touch) {
            // Update reusable buffer to avoid object allocations
            mouseCoordinateBuffer.clientX = touch.clientX;
            mouseCoordinateBuffer.clientY = touch.clientY;
            mouseCoordinateBuffer.offsetX = touch.clientX; // Touch events don't have offsetX/Y
            mouseCoordinateBuffer.offsetY = touch.clientY;
            
            mouseEventContexts.forEach(({ managedInstance }) => {
                safeInvokeBlazorMethod(managedInstance, 'MouseMoved', mouseCoordinateBuffer);
            });
        }
    };
    
    /**
     * Handles global window resize events
     */
    const handleGlobalResize = (): void => {
        if (resizeEventContexts.length !== canvasContexts.size) {
            refreshContextCaches();
        }
        
        const { innerWidth, innerHeight } = window;
        resizeEventContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'Resized', innerWidth, innerHeight);
        });
    };
    
    // ========================================
    // PUBLIC API IMPLEMENTATION
    // ========================================
    
    /**
     * Initializes a canvas element with event handlers and context
     */
    const initCanvas = (
        canvasId: string,
        blazorInstance: DotNetObjectReference,
        contextOptions: CanvasContextOptions = DEFAULT_CANVAS_OPTIONS
    ): void => {
        const canvas = document.getElementById(canvasId) as HTMLCanvasElement;
        if (!canvas) {
            console.warn(`Canvas element with ID '${canvasId}' not found`);
            return;
        }
        
        // Create event handlers for this canvas
        const eventHandlers = {
            wheel: createCanvasEventHandler(blazorInstance, 'wheel'),
            mousedown: createCanvasEventHandler(blazorInstance, 'mousedown'),
            mouseup: createCanvasEventHandler(blazorInstance, 'mouseup'),
            touchstart: createCanvasEventHandler(blazorInstance, 'touchstart'),
            touchend: createCanvasEventHandler(blazorInstance, 'touchend')
        } as const;
        
        // Attach event listeners with non-passive mode for performance
        const eventOptions: AddEventListenerOptions = { passive: false };
        Object.entries(eventHandlers).forEach(([eventType, handler]) => {
            canvas.addEventListener(eventType, handler, eventOptions);
        });
        
        // Get 2D rendering context
        const renderingContext = canvas.getContext('2d', contextOptions);
        if (!renderingContext) {
            console.error(`Failed to get 2D rendering context for canvas '${canvasId}'`);
            return;
        }
        
        // Store context info
        canvasContexts.set(canvasId, {
            id: canvasId,
            context: renderingContext,
            managedInstance: blazorInstance,
            contextOptions
        });

        requestAnimationFrame(onFrameUpdate);
    };
    
    /**
     * Handles frame update loop with RequestAnimationFrame
     */
    const onFrameUpdate = (timestamp: number): void => {
        // Refresh context cache if needed
        if (cachedContexts.length !== canvasContexts.size) {
            cachedContexts = Array.from(canvasContexts.values());
        }
        
        // Notify all contexts of frame update
        cachedContexts.forEach(({ managedInstance }) => {
            safeInvokeBlazorMethod(managedInstance, 'UpdateFrame', timestamp);
        });
        
        // Schedule next frame
        requestAnimationFrame(onFrameUpdate);
    };
    
    /**
     * Creates a new ImageData object and returns its ID
     */
    const createImageData = (canvasId: string, width: number, height: number): number | null => {
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
    
    /**
     * Updates ImageData with new pixel data and renders to canvas
     */
    const putImageData = (
        canvasId: string, 
        imageId: number, 
        pixelData: Uint8ClampedArray, 
        x: number, 
        y: number
    ): void => {
        const contextInfo = canvasContexts.get(canvasId);
        const imageData = imageDataCache.get(imageId);
        
        if (!contextInfo || !imageData) {
            console.warn(`Invalid canvas '${canvasId}' or image '${imageId}'`);
            return;
        }
        
        imageData.data.set(pixelData);
        contextInfo.context.putImageData(imageData, x, y);
    };
    
    /**
     * Processes a batch of canvas operations efficiently
     */
    const processBatch = (canvasId: string, operations: readonly BatchOperation[]): void => {
        if (!operations?.length) return;
        
        const contextInfo = canvasContexts.get(canvasId);
        if (!contextInfo) {
            console.warn(`Canvas context '${canvasId}' not found`);
            return;
        }
        
        const { context } = contextInfo;
        
        // Process all operations in sequence
        operations.forEach(({ methodName, args, isProperty }) => {
            if (isProperty) {
                setContextProperty(context, methodName, args as PropertyValue);
            } else {
                invokeContextMethod(context, methodName, args as unknown[]);
            }
        });
    };
    
    /**
     * Directly calls a single method on the canvas context
     */
    const directCall = (
        canvasId: string, 
        methodName: string, 
        parameters?: unknown[]
    ): unknown => {
        const contextInfo = canvasContexts.get(canvasId);
        if (!contextInfo) {
            console.warn(`Canvas context '${canvasId}' not found`);
            return null;
        }
        
        return invokeContextMethod(contextInfo.context, methodName, parameters);
    };
    
    /**
     * Removes a canvas context and cleans up resources
     */
    const removeContext = (canvasId: string): boolean => {
        return canvasContexts.delete(canvasId);
    };
    
    /**
     * Resizes a canvas and notifies Blazor of the change
     */
    const resizeCanvas = (canvasId: string, width: number, height: number): void => {
        const contextInfo = canvasContexts.get(canvasId);
        const canvas = document.getElementById(canvasId) as HTMLCanvasElement | null;
        
        if (!contextInfo || !canvas) {
            console.warn(`Canvas '${canvasId}' not found for resize operation`);
            return;
        }
        
        // Update canvas dimensions (triggers context reset)
        canvas.width = width;
        canvas.height = height;
        
        // Re-acquire context with original options
        const newContext = canvas.getContext('2d', contextInfo.contextOptions);
        if (newContext) {
            contextInfo.context = newContext;
            safeInvokeBlazorMethod(contextInfo.managedInstance, 'Resized', width, height);
        }
    };
    
    // ========================================
    // GLOBAL EVENT SETUP
    // ========================================
    
    // Set up global event listeners with passive mode for better performance
    const globalEventOptions: AddEventListenerOptions = { passive: true };
    
    addEventListener('keyup', handleGlobalKeyUp, globalEventOptions);
    addEventListener('keydown', handleGlobalKeyDown, globalEventOptions);
    addEventListener('mousemove', handleGlobalMouseMove, globalEventOptions);
    addEventListener('touchmove', handleGlobalTouchMove, globalEventOptions);
    addEventListener('resize', handleGlobalResize, globalEventOptions);
    
    // ========================================
    // API EXPORT
    // ========================================
    
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
