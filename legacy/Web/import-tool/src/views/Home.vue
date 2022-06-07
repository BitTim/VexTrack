<template>
    <div>
        <h1 class="font-bold text-2xl">Home</h1>
        <p>All possible files that can be imported are located in <code>%localappdata%/VexTrack/</code></p>
        
        <div class="mt-4 grid grid-cols-2">
            <div class="pr-2">
                <h2 class="font-bold text-xl mb-2">Data Import</h2>
                <!-- <label class="" for="form_dataFile_input">Upload file</label>
                <input id="form_dataFile_input" type="file" accept=".json" ref="fileInput" @change="fileChanged" />
                <button @click.prevent="importClicked">Import</button> -->

                <div
                    @dragover="dataDragging = true" @dragleave="dataDragging = false"
                    class="flex flex-col border-2 px-8 py-8 items-center border-dashed border-LightShade dark:border-DarkShade rounded-lg">
                    <!-- :class="[dataDragging ? 'border-gradient'" -->
                    <p class="text-xl">Drop data.json file to upload</p>
                    <p class="mb-2">or</p>
                    <label class="bg-gradient-to-br from-Aqua1 to-Aqua2 px-4 h-9 inline-flex items-center rounded-lg shadow-md text-sm font-medium text-white">
                        Select data.json file
                        <input type="file" name="dataFile" accept=".json" ref="dataFileInput" @change="dataFileChanged" class="sr-only" />
                    </label>
                    <p class="text-xs mt-4">Please note that only a valid data.json from version v1.86 or v1.87 is accepted</p>
                </div>

                <div class="grid grid-cols-2 my-4">
                    <button @click.prevent="dataResetClicked" class="bg-LightShade dark:bg-DarkShade px-4 h-9 mr-2 items-center rounded-lg shadow-md text-sm font-medium">Reset</button>
                    <button @click.prevent="dataImportClicked" class="bg-gradient-to-br from-Aqua1 to-Aqua2 px-4 h-9 ml-2 items-center rounded-lg shadow-md text-sm font-medium text-white">Import</button>
                </div>
                <br>

                <DataPreviewComponent v-bind="preview"/>
            </div>
            <div class="pl-2">
                <h2 class="font-bold text-xl mb-2">Settings Import</h2>
                <div @dragover="settingsDragging = true" @dragleave="settingsDragging = false" class="flex flex-col border-2 px-8 py-8 items-center border-dashed border-LightShade dark:border-DarkShade rounded-lg">
                    <p class="text-xl">Drop settings.json file to upload</p>
                    <p class="mb-2">or</p>
                    <label class="bg-gradient-to-br from-Aqua1 to-Aqua2 px-4 h-9 inline-flex items-center rounded-lg shadow-md text-sm font-medium text-white">
                        Select settings.json file
                        <input type="file" name="settingsFile" accept=".json" ref="settingsFileInput" @change="settingsFileChanged" class="sr-only" />
                    </label>
                    <p class="text-xs mt-4">Please note that only a valid settings.json from version v1.86 or v1.87 is accepted</p>
                </div>
                
                <div class="grid grid-cols-2 my-4">
                    <button @click.prevent="settingsResetClicked" class="bg-LightShade dark:bg-DarkShade px-4 h-9 mr-2 items-center rounded-lg shadow-md text-sm font-medium">Reset</button>
                    <button @click.prevent="settingsImportClicked" class="bg-gradient-to-br from-Aqua1 to-Aqua2 px-4 h-9 ml-2 items-center rounded-lg shadow-md text-sm font-medium text-white">Import</button>
                </div>
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

export default {
    setup: () => {
		const preview: any = ref(defaultPreview());
        const dataFileInput = ref();

        const dataDragging = ref(false);
        const settingsDragging = ref(true);

        var dataFile: File | null = null;

        const dataFileChanged = async (event: any) => {
            dataFile = event?.target?.files[0];

            if(dataFile === null) alert("Invalid file");
            preview.value = parsePreview(await loadFile(dataFile as File));
        };
        
        const dataImportClicked = async () => {
            if(dataFile === null) alert("Invalid file");
            await importData(await loadFile(dataFile as File))
            await dataResetClicked();
        };

        const dataResetClicked = async () => {
            dataFile = null;
            dataFileInput.value.value = '';
            preview.value = defaultPreview();
        };
        
        return { dataDragging, settingsDragging, dataFileChanged, dataImportClicked, dataResetClicked, preview, dataFileInput };
    },

    components: { DataPreviewComponent }
}
</script>