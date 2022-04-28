<template>
	<div>
		<h1>Home</h1>
		<p>Please select your data.json here. It is located in %localappdata%/VexTrack/data.json</p>
		<input type="file" accept=".json" @change="fileChanged" />
		<button @click.prevent="importClicked">Import</button>
		<DataPreviewComponent preview="preview"/>
	</div>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, parsePreview, parseData } from "../core"
import DataPreviewComponent from "../components/DataPreviewComponent.vue";
import { GameModel } from "../models/GameModel";

export default {
    setup: () => {
		var preview: any;

        const fileChanged = async (event: any) => {
            loadFile(event?.target?.files[0]);
            preview = parsePreview();
        };
        const importClicked = async () => {
            parseData();
        };
        return { fileChanged, importClicked, preview };
    },

    components: { DataPreviewComponent }
}
</script>