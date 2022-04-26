<template>
	<div>
		<h1>Home</h1>
		<form>
			<p>Please select your data.json here. It is located in %localappdata%/VexTrack/data.json</p>
			<input ref="dataFile" type="file" @change="fileChanged()" />
			<input type="submit" accept=".json" @click.prevent="fileSelected()" />
		</form>
	</div>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, parseData } from "../core"

export default {
	setup: () => {
		const dataFile = ref();

		const fileChanged = async () => {
			loadFile(dataFile.value)
		}

		const fileSelected = async () => {
			parseData()
		}

		return { dataFile, fileChanged, fileSelected }
	}
}
</script>