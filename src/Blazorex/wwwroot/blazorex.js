window.Blazorex = (() => {
    const _canvases = [],
        _contexts = [],
        _refs = [];

    const initCanvas = (id) => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            return;
        }
        _canvases[id] = canvas;
        _contexts[id] = canvas.getContext("2d");
    }, getRef = (ref) => {
        const pId = `_bl_${ref.id}`,
            elem = _refs[pId] || document.querySelector(`[${pId}]`);
        _refs[pId] = elem;
        return elem;
    }, callCanvasMethod = (rawCtxId, rawMethod, rawParams) => {
        const ctxId = BINDING.conv_string(rawCtxId),
            ctx = _contexts[ctxId];
        if (!ctx) {
            return;
        }
        const method = BINDING.conv_string(rawMethod),
            jsonParams = BINDING.conv_string(rawParams),
            params = JSON.parse(jsonParams);

        for (let p in params) {
            if (params[p].isRef) {
                params[p] = getRef(params[p]);
            }
        }

        ctx[method](...params);
    }, setCanvasProperty = (rawCtxId, rawProp, rawValue) => {
        const ctxId = BINDING.conv_string(rawCtxId),
            ctx = _contexts[ctxId];
        if (!ctx) {
            return;
        }
        const property = BINDING.conv_string(rawProp),
            jsonValue = BINDING.conv_string(rawValue);

        ctx[property] = jsonValue;
    };

    return {
        initCanvas,
        callCanvasMethod,
        setCanvasProperty
    };
})();