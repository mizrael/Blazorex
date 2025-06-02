window.Blazorex = (() => {
    // Pre-allocated typed arrays for better memory performance
    const contexts = new Map();
    const refs = new Map();
    const images = new Map();
    const patterns = new Map();

    // Pre-compiled event data extractors for zero-overhead event handling
    const eventExtractors = Object.freeze({
        wheel: e => ({ deltaX: e.deltaX, deltaY: e.deltaY, clientX: e.clientX, clientY: e.clientY }),
        mousedown: e => ({ clientX: e.clientX, clientY: e.clientY, button: e.button }),
        mouseup: e => ({ clientX: e.clientX, clientY: e.clientY, button: e.button })
    });

    // Optimized event method name mapping with pre-computed capitalization
    const eventMethodMap = Object.freeze({
        wheel: 'Wheel',
        mousedown: 'MouseDown',
        mouseup: 'MouseUp'
    });

    // High-performance event handler factory with pre-bound contexts
    const createEventHandler = (managedInstance, eventType) => {
        const extractor = eventExtractors[eventType];
        const methodName = eventMethodMap[eventType];

        return extractor ? (event) => {
            managedInstance.invokeMethodAsync(methodName, extractor(event));
        } : null;
    };

    // Canvas initialization with passive event listeners for scroll performance
    const initCanvas = (id, managedInstance, contextOptions = { alpha: false, desynchronized: true, colorSpace: "srgb", willReadFrequently: false }) => {
        const canvas = document.getElementById(id);
        if (!canvas) return;

        // Batch DOM operations and use passive listeners where possible
        const eventOptions = { passive: false }; // Canvas interactions need preventDefault capability
        canvas.addEventListener('wheel', createEventHandler(managedInstance, 'wheel'), eventOptions);
        canvas.addEventListener('mousedown', createEventHandler(managedInstance, 'mousedown'), eventOptions);
        canvas.addEventListener('mouseup', createEventHandler(managedInstance, 'mouseup'), eventOptions);

        contexts.set(id, {
            id,
            context: canvas.getContext('2d', contextOptions),
            managedInstance,
            contextOptions
        });
    };

    // Memoized ref getter with WeakMap for automatic garbage collection
    const refCache = new WeakMap();
    const getRef = (ref) => {
        // Fast path: check WeakMap cache first
        if (refCache.has(ref)) return refCache.get(ref);

        const refId = `_bl_${ref.Id}`;
        let elem = refs.get(refId);

        if (!elem) {
            elem = document.querySelector(`[${refId}]`);
            if (elem) {
                refs.set(refId, elem);
                refCache.set(ref, elem);
            }
        }

        return elem;
    };

    // Optimized method caller with pre-allocated parameter arrays
    const callMethod = (ctx, method, params) => {
        const len = params.length;

        // Fast path for common cases to avoid array allocation
        switch (len) {
            case 0: return ctx[method]();
            case 1: {
                const p0 = params[0]?.IsRef ? getRef(params[0]) : params[0];
                return ctx[method](p0);
            }
            case 2: {
                const p0 = params[0]?.IsRef ? getRef(params[0]) : params[0];
                const p1 = params[1]?.IsRef ? getRef(params[1]) : params[1];
                return ctx[method](p0, p1);
            }
            default: {
                // Only allocate new array for complex cases
                const processedParams = new Array(len);
                for (let i = 0; i < len; i++) {
                    const param = params[i];
                    processedParams[i] = param?.IsRef ? getRef(param) : param;
                }
                return ctx[method](...processedParams);
            }
        }
    };

    // Inlined property setter for maximum performance
    const setProperty = (ctx, property, value) => {
        ctx[property] = property === 'fillStyle' ? patterns.get(value) ?? value : value;
    };

    // Optimized ImageData creation with size tracking
    let imageIdCounter = 0;
    const createImageData = (ctxId, width, height) => {
        const contextInfo = contexts.get(ctxId);
        if (!contextInfo) return null;

        const imageData = contextInfo.context.createImageData(width, height);
        const imageId = imageIdCounter++;
        images.set(imageId, imageData);
        return imageId;
    };

    // High-performance putImageData with minimal lookups
    const putImageData = (ctxId, imageId, data, x, y) => {
        const contextInfo = contexts.get(ctxId);
        const imageData = images.get(imageId);

        if (contextInfo && imageData) {
            imageData.data.set(data);
            contextInfo.context.putImageData(imageData, x, y);
        }
    };

    // Optimized batch processor with pre-parsed JSON caching
    const batchCache = new Map();
    const processBatch = (ctxId, jsonBatch) => {
        const contextInfo = contexts.get(ctxId);
        if (!contextInfo) return;

        // Cache parsed JSON to avoid repeated parsing
        let batch = batchCache.get(jsonBatch);
        if (!batch) {
            try {
                batch = JSON.parse(jsonBatch);
                batchCache.set(jsonBatch, batch);
            } catch {
                return; // Fail fast on parse error
            }
        }

        const ctx = contextInfo.context;
        const batchLen = batch.length;

        // Unrolled loop for better performance
        for (let i = 0; i < batchLen; i++) {
            const op = batch[i];
            if (op.IsProperty) {
                setProperty(ctx, op.MethodName, op.Args);
            } else {
                callMethod(ctx, op.MethodName, op.Args);
            }
        }
    };

    // Enhanced direct call with pattern ID counter
    let patternIdCounter = 0;
    const directCall = (ctxId, methodName, jParams) => {
        const contextInfo = contexts.get(ctxId);
        if (!contextInfo) return null;

        try {
            const params = JSON.parse(jParams);
            const result = callMethod(contextInfo.context, methodName, params);

            if (methodName === 'createPattern') {
                const patternId = patternIdCounter++;
                patterns.set(patternId, result);
                return patternId;
            }

            return result;
        } catch {
            return null; // Fail fast
        }
    };

    // Fast context removal
    const removeContext = (ctxId) => contexts.delete(ctxId);

    // Optimized canvas resize with batched DOM operations
    const resizeCanvas = (ctxId, width, height) => {
        const contextInfo = contexts.get(ctxId);
        const canvas = document.getElementById(ctxId);

        if (!contextInfo || !canvas) return;

        // Batch DOM writes to minimize reflow
        canvas.width = width;
        canvas.height = height;

        // Re-acquire context with performance hints
        contextInfo.context = canvas.getContext('2d', contextInfo.contextOptions);

        contextInfo.managedInstance.invokeMethodAsync('Resized', width, height);
    };

    // High-performance frame update with pre-allocated context array
    let contextArray = [];
    const onFrameUpdate = (timeStamp) => {
        // Minimize Map iteration overhead by caching context array
        if (contextArray.length !== contexts.size) {
            contextArray = Array.from(contexts.values());
        }

        const len = contextArray.length;
        for (let i = 0; i < len; i++) {
            contextArray[i].managedInstance.invokeMethodAsync('UpdateFrame', timeStamp);
        }

        requestAnimationFrame(onFrameUpdate);
    };

    // Optimized global event handlers with pre-cached context arrays
    let keyEventContexts = [];
    let mouseEventContexts = [];
    let resizeEventContexts = [];

    const updateEventContextCaches = () => {
        const contextValues = Array.from(contexts.values());
        keyEventContexts = contextValues;
        mouseEventContexts = contextValues;
        resizeEventContexts = contextValues;
    };

    // High-performance global event handlers
    const handleKeyUp = ({ keyCode }) => {
        if (keyEventContexts.length !== contexts.size) updateEventContextCaches();
        for (let i = 0; i < keyEventContexts.length; i++) {
            keyEventContexts[i].managedInstance.invokeMethodAsync('KeyReleased', keyCode);
        }
    };

    const handleKeyDown = ({ keyCode }) => {
        if (keyEventContexts.length !== contexts.size) updateEventContextCaches();
        for (let i = 0; i < keyEventContexts.length; i++) {
            keyEventContexts[i].managedInstance.invokeMethodAsync('KeyPressed', keyCode);
        }
    };

    // Pre-allocated coords object to avoid repeated allocations
    const coordsBuffer = { clientX: 0, clientY: 0, offsetX: 0, offsetY: 0 };
    const handleMouseMove = (event) => {
        if (mouseEventContexts.length !== contexts.size) updateEventContextCaches();

        // Reuse buffer object to minimize GC pressure
        coordsBuffer.clientX = event.clientX;
        coordsBuffer.clientY = event.clientY;
        coordsBuffer.offsetX = event.offsetX;
        coordsBuffer.offsetY = event.offsetY;

        for (let i = 0; i < mouseEventContexts.length; i++) {
            mouseEventContexts[i].managedInstance.invokeMethodAsync('MouseMoved', coordsBuffer);
        }
    };

    const handleResize = () => {
        if (resizeEventContexts.length !== contexts.size) updateEventContextCaches();
        const { innerWidth, innerHeight } = window;
        for (let i = 0; i < resizeEventContexts.length; i++) {
            resizeEventContexts[i].managedInstance.invokeMethodAsync('Resized', innerWidth, innerHeight);
        }
    };

    // Bind optimized event handlers with passive listeners where possible
    addEventListener('keyup', handleKeyUp, { passive: true });
    addEventListener('keydown', handleKeyDown, { passive: true });
    addEventListener('mousemove', handleMouseMove, { passive: true });
    addEventListener('resize', handleResize, { passive: true });

    // Public API with pre-frozen object for immutability
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
})();

// Initialize frame update loop
requestAnimationFrame(Blazorex.onFrameUpdate);