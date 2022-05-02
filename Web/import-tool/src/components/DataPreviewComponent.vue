<template>
	<div>
		<h4>Preview</h4>
		<h5>{{ preview.season_name }}</h5>
	</div>
</template>

<script lang="ts">
import { defaultPreview } from "../core"

export default {
	props: {
		preview: {
			type: Object,
			required: true,
			default: defaultPreview()
		}
	}
}
</script>