package com.bittim.vextrack.core

import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.firestore.DocumentSnapshot
import com.google.firebase.firestore.FirebaseFirestore
import java.util.*
import kotlin.reflect.KFunction1

class DataHandler
{
    companion object
    {
        val auth = FirebaseAuth.getInstance()
        var uid: String? = auth.currentUser?.uid

        var subscribers: MutableList<(DocumentSnapshot?) -> Unit> = mutableListOf()
        var data = null

        fun loadData()
        {
            val docRef = FirebaseFirestore.getInstance().collection("users").document(uid as String)
            docRef.addSnapshotListener { snapshot, e ->
                if (e != null) {
                    // Listen failed
                    return@addSnapshotListener
                }

                for (sub in subscribers) {
                    sub(snapshot)
                }
            }
        }

        fun subscribe(listener: (DocumentSnapshot?) -> Unit)
        {
            subscribers.add(listener)
        }
    }
}