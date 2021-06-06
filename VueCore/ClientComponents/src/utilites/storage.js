const saveItem = (key, item) => {
    console.log('saveItem', key);
    console.log('saveItem', item);
    localStorage.setItem(key, item);
}
const saveObject = (key, obj) => {
    const item = JSON.stringify(obj);
    console.log(item);
    saveItem(key, item);
}
const removeKey = (key) => {
    localStorage.removeItem(key);
}
const getItem = (key) => {
    return localStorage.getItem(key);
}
const getObject = (key) => {
    const item = getItem(key);
    console.log(item);
    if(item) {
        return JSON.parse(item);
    }
    return {};
}

export {saveItem, saveObject, removeKey, getItem, getObject};

