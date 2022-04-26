var data: null | Object = null

const loadFile = (file: File) => {
    const reader = new FileReader();

    reader.onload = e => {
        const content = e.target?.result?.toString();
        if (content === null) return;

        const result = JSON.parse(content as string);
        data = result
    }
    reader.readAsText(file);
}

const parseData = () => {
    console.log(JSON.stringify(data, null, 4));
}

export { loadFile, parseData }