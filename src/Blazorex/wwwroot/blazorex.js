window.Blazorex = (() => {
    const _contexts = [],
        _refs = [],
        _patterns = [];

    const initCanvas = (id, managedInstance) => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            return;
        }



        _contexts[id] = {
            id: id,
            context: canvas.getContext("2d"),
            managedInstance
        };
    }, getRef = (ref) => {
        const pId = `_bl_${ref.Id}`,
            elem = _refs[pId] || document.querySelector(`[${pId}]`);
        _refs[pId] = elem;
        return elem;
    }, callMethod = (ctx, method, params) => {
        for (let p in params) {
            if (params[p] != null && params[p].IsRef) {
                params[p] = getRef(params[p]);
            }
        }

        const result = ctx[method](...params);
        return result;
    },
    setProperty = (ctx, property, value) => {
        const propValue = (property == 'fillStyle' ? _patterns[value] || value : value);
        ctx[property] = propValue;
    },
    onFrameUpdate = (timeStamp) => {
        for (let ctx in _contexts) {
            _contexts[ctx].managedInstance.invokeMethodAsync('UpdateFrame', timeStamp);
        }
        window.requestAnimationFrame(onFrameUpdate);
    },
    processBatch = (rawCtxId, rawBatch) => {
        const ctxId = BINDING.conv_string(rawCtxId),
            ctx = _contexts[ctxId].context;
        if (!ctx) {
            return;
        }
        const jsonBatch = BINDING.conv_string(rawBatch),
            batch = JSON.parse(jsonBatch);

        for (let i in batch) {
            const op = batch[i];
            if (op.IsProperty)
                setProperty(ctx, op.MethodName, op.Args);
            else
                callMethod(ctx, op.MethodName, op.Args);
        }
    },
    directCall = (rawCtxId, rawMethodName, rawParams) => {
        const ctxId = BINDING.conv_string(rawCtxId),
            ctx = _contexts[ctxId].context;
        if (!ctx) {
            return;
        }
        const methodName = BINDING.conv_string(rawMethodName),
            jParams = BINDING.conv_string(rawParams),
            params = JSON.parse(jParams),
            result = callMethod(ctx, methodName, params);            

        if (methodName == 'createPattern') {
            const patternId = _patterns.length;
            _patterns.push(result);
            return BINDING.js_to_mono_obj(patternId);
        }

        return BINDING.js_to_mono_obj(result);
    };

    window.onkeyup = (e) => {
        for (let ctx in _contexts) {
            _contexts[ctx].managedInstance.invokeMethodAsync('KeyReleased', e.keyCode);
        }
    };
    window.onkeydown = (e) => {
        for (let ctx in _contexts) {
            _contexts[ctx].managedInstance.invokeMethodAsync('KeyPressed', e.keyCode);
        }
    };
    window.onmousemove = (e) => {
        const coords = {
            X: e.offsetX,
            Y: e.offsetY
        };
        for (let ctx in _contexts) {
            _contexts[ctx].managedInstance.invokeMethodAsync('MouseMoved', coords);
        }
    };

    return {
        initCanvas,
        onFrameUpdate,
        processBatch,
        directCall
    };
})();

window.requestAnimationFrame(Blazorex.onFrameUpdate);