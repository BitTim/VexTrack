package com.bittim.vextrack

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.bittim.vextrack.databinding.ActivityMainBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser

class MainActivity : AppCompatActivity() {
    private lateinit var binding: ActivityMainBinding
    private lateinit var auth: FirebaseAuth

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        auth = FirebaseAuth.getInstance()

        initTitle()
        initButtons()
    }

    override fun onStart()
    {
        super.onStart()

        val user: FirebaseUser? = auth.currentUser
        if (user == null)
        {
            startActivity(Intent(this@MainActivity, WelcomeActivity::class.java))
            finish()
        }

        user?.reload()?.addOnCompleteListener {
            if(!it.isSuccessful)
            {
                startActivity(Intent(this@MainActivity, WelcomeActivity::class.java))
                finish()
            }
        }
    }



    // ================================
    //  Initializers
    // ================================

    private fun initTitle() { binding.title.setText(R.string.home_frag_name) }

    private fun initButtons()
    {
        binding.epilogueButton.setOnClickListener { onEpilogueButtonClicked() }
    }

    // ================================
    //  Button Handlers
    // ================================

    private fun onEpilogueButtonClicked()
    {
        binding.epilogueButton.isActivated = !binding.epilogueButton.isActivated
    }
}