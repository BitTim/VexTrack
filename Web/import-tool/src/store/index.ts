import { defineStore, acceptHMRUpdate } from "pinia";
import router from "../router";
import { auth } from "../firebase"
import { authError } from "../firebase/errors";

import { createUserWithEmailAndPassword, User } from "firebase/auth";
import { signInWithEmailAndPassword } from "firebase/auth";
import { updateProfile } from "firebase/auth";
import { signOut } from "firebase/auth";
import { FirebaseError } from "firebase/app";

export const useUserStore = defineStore({
	id: "userStore",

	state: () => ({
		user: null as User | null
	}),

	getters: {
		loggedIn: (state) => state.user !== null
	},

	actions: {
		setUser(user: User)
		{
			this.$patch({user: user})
		},

		clearUser()
		{
			this.$patch({user: null})
		},



		async login (email: string, password: string) 
		{
			try {
				await signInWithEmailAndPassword(auth, email, password)

			} catch (error: FirebaseError | any) {
				authError(error)
				return error
			}

			this.setUser(auth.currentUser as User);
			router.push("/")
		},

		async signup (username: string, email: string, password: string)
		{
			try {
				await createUserWithEmailAndPassword(auth, email, password)
				await updateProfile(auth.currentUser as User, {displayName: username})

			} catch (error: FirebaseError | any) {
				authError(error)
				return error
			}

			this.setUser(auth.currentUser as User);
			router.push("/")
		},

		async signout ()
		{
			await signOut(auth)

			this.clearUser()
			router.push("/login")
		},

		fetchUser()
		{
			auth.onAuthStateChanged(async user => {
				if (user === null) {
					this.clearUser();
				} else {
					this.setUser(user);

					router.isReady().then(() => {
						if(!router.currentRoute.value.meta.requiresAuth) router.push("/");
					});
				}
			})
		}
	},
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useUserStore, import.meta.hot))
}