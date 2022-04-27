const NUM_PREVIEW_HISTORY_ENTRIES = 10;
const NUM_PREVIEW_SEASON_ENTRIES = 3;

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




const parsePreview = () => {
    
}


const parseData = () => {
    console.log(JSON.stringify(data, null, 4));
}

export { loadFile, parsePreview, parseData }