import { FirebaseError } from "firebase/app"

const authError = (error : FirebaseError) => {
	switch(error.code) {
		case "auth/user-not-found":
			alert("User not found")
			break

		case "auth/wrong-password":
			alert("Wrong password")
			break

		case "auth/email-already-in-use":
			alert("E-Mail already in use")
			break
		
		case "auth/invalid-email":
			alert("Invalid E-Mail")
			break
		
		case "auth/operation-not-allowed":
			alert("Operation not allowed")
			break

		case "auth/weak-password":
			alert("Weak password")
			break

		default:
			alert(error.message)
	}
}

export { authError }