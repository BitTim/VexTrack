import { defineStore } from "pinia";
import router from "../router";
import { auth } from "../firebase"
import { authError } from "../firebase/errors";

import { createUserWithEmailAndPassword } from "firebase/auth";
import { signInWithEmailAndPassword } from "firebase/auth";
import { updateProfile } from "firebase/auth";
import { signOut } from "firebase/auth";

export const useUserStore = defineStore({
	state: () => ({
		user: null
	}),

	getters: {
		loggedIn: (state) => state.user !== null
	},

	actions: {
		setUser(user) {
			this.user = user
		},

		clearUser() {
			this.user = null
		},



		async login (details) {
			const { email, password } = details;

			try {
				await signInWithEmailAndPassword(auth, email, password)

			} catch (error) {
				authError(error)
				return error
			}

			this.setUser(auth.currentUser);
			router.push("/tool")
		},

		async signup (details) {
			const { username, email, password } = details;

			try {
				await createUserWithEmailAndPassword(auth, email, password)
				await updateProfile(auth.currentUser, {displayName: username})

			} catch (error) {
				authError(error)
				return error
			}

			this.setUser(auth.currentUser);
			router.push("/tool")
		},

		async signout () {
			await signOut(auth)

			this.clearUser()
			router.push("/")
		}
	}
})