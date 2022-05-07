<template>
    <div>
        <h1 class="font-bold text-2xl">Home</h1>
        <p>All possible files that can be imported are located in <code>%localappdata%/VexTrack/</code></p>
        
        <div class="mt-4 grid grid-cols-2">
            <div>
                <h2 class="font-bold text-xl">Data Import</h2>
                <p>Please select your data.json here</p>
                <input type="file" accept=".json" ref="fileInput" @change="fileChanged" />
                <button @click.prevent="importClicked">Import</button>
                <br>

                <DataPreviewComponent v-bind="preview"/>
            </div>
            <div>
                <h2 class="font-bold text-xl">Settings Import</h2>
                <p>Please select your settings.json here</p>
            </div>
        </div>

        <br>
        <br>
        <br>
        <br>
    </div>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, defaultPreview, parsePreview, importData } from "../core"
import DataPreviewComponent from "../components/DataPreviewComponent.vue";
import { GameModel } from "../models/GameModel";

export default {
    setup: () => {
		const preview: any = ref(defaultPreview());
        const fileInput = ref();

        var file: File | null = null;

        const fileChanged = async (event: any) => {
            file = event?.target?.files[0];

            if(file === null) alert("Invalid file");
            preview.value = parsePreview(await loadFile(file as File));
        };
        
        const importClicked = async () => {
            if(file === null) alert("Invalid file");
            await importData(await loadFile(file as File))

            file = null;
            fileInput.value.value = '';
            preview.value = defaultPreview();
        };
        
        return { fileChanged, importClicked, preview, fileInput };
    },

    components: { DataPreviewComponent }
}
</script>