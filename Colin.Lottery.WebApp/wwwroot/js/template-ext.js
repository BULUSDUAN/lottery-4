template.defaults.imports.toFixed=function (number,length) {
    return number.toFixed((length==undefined||length==null||length==NaN)?2:length);
};