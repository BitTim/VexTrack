package com.bittim.vextrack

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.bittim.vextrack.databinding.ActivityMainBinding

class MainActivity : AppCompatActivity() {
    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initTitle()
        initButtons()
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