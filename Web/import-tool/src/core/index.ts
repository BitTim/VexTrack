var data = null;

const loadFile = (file: File) => {
    const reader = new FileReader();

    reader.onload = e => {
        const result = JSON.parse(e.target.result.toString());
        parseData(result);
    }
    reader.readAsText(file);
}

const parseData = (json: Object) => {

}

export { loadFile, parseData }