import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getAuth } from "firebase/auth";
import { getFirestore } from "firebase/firestore";

const firebaseConfig = {
	apiKey: "AIzaSyA5Tdvil2nOc_97z_6ANk7bXITSAcvr7ko",
	authDomain: "vextrack-c530b.firebaseapp.com",
	projectId: "vextrack-c530b",
	storageBucket: "vextrack-c530b.appspot.com",
	messagingSenderId: "975706202791",
	appId: "1:975706202791:web:6552401bb9a333b78af182",
	measurementId: "G-JT8KXNWF7Y"
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);
const auth = getAuth(app);
const analytics = getAnalytics(app);

export { db, auth, analytics }