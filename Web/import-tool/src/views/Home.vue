<template>
	<div>
		<h1>Home</h1>
		<p>Please select your data.json here. It is located in %localappdata%/VexTrack/data.json</p>
		<input type="file" accept=".json" @change="fileChanged" />
		<button @click.prevent="importClicked">Import</button>
        <br>

		<DataPreviewComponent v-bind="preview"/>
	</div>

    <br>
    <br>
    <br>
    <br>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, defaultPreview, parsePreview, importData } from "../core"
import DataPreviewComponent from "../components/DataPreviewComponent.vue";
import { GameModel } from "../models/GameModel";

export default {
    setup: () => {
		const preview: any = ref(defaultPreview());

        var file: File | null = null;

        const fileChanged = async (event: any) => {
            file = event?.target?.files[0];

            if(file === null) alert("Invalid file");
            preview.value = parsePreview(await loadFile(file as File));
        };
        const importClicked = async () => {
            if(file === null) alert("Invalid file");
            await importData(await loadFile(file as File))
        };
        return { fileChanged, importClicked, preview };
    },

    components: { DataPreviewComponent }
}
</script>