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
import { loadFile, defaultPreview, parseData } from "../core"
import DataPreviewComponent from "../components/DataPreviewComponent.vue";
import { GameModel } from "../models/GameModel";

export default {
    setup: () => {
		const preview: any = ref(defaultPreview());

        const fileChanged = async (event: any) => {
            preview.value = await loadFile(event?.target?.files[0]);
        };
        const importClicked = async () => {
            //parseData();
        };
        return { fileChanged, importClicked, preview };
    },

    components: { DataPreviewComponent }
}
</script>