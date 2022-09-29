window.Blazorex = (() => {
    const _contexts = [],
        _refs = [];

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

        ctx[method](...params);
    },
    setProperty = (ctx, property, value) => {
        ctx[property] = value;
    },
    onFrameUpdate = (timeStamp) => {
        for (let ctx in _contexts) {
            _contexts[ctx].managedInstance.invokeMethodAsync('__BlazorexGameLoop', timeStamp);
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
    };

    return {
        initCanvas,
        onFrameUpdate,
        processBatch
    };
})();

window.requestAnimationFrame(Blazorex.onFrameUpdate);