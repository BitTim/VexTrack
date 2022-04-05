package com.bittim.vextrack

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.bittim.vextrack.databinding.ActivityWelcomeBinding
import com.bittim.vextrack.fragments.LogInFragment
import com.bittim.vextrack.fragments.SignUpFragment
import com.bittim.vextrack.fragments.adapters.WelcomeViewPagerAdapter

class WelcomeActivity : AppCompatActivity() {
    private lateinit var binding: ActivityWelcomeBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityWelcomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initPages()
        initButtons()
    }



    // ================================
    //  Initializers
    // ================================

    private fun initPages()
    {
        val adapter = WelcomeViewPagerAdapter(supportFragmentManager)
        adapter.addItem(LogInFragment())
        adapter.addItem(SignUpFragment())

        binding.welcomeViewPager.adapter = adapter
        binding.welcomeViewPager.currentItem = 0
    }

    private fun initButtons()
    {
        binding.welcomeSignUpButton.setOnClickListener { onWelcomeSignUpButtonClicked() }
        binding.welcomeLogInButton.setOnClickListener { onWelcomeLogInButtonClicked() }
    }



    // ================================
    //  Initializers
    // ================================

    private fun onWelcomeLogInButtonClicked()
    {
        binding.welcomeViewPager.currentItem = 0
    }

    private fun onWelcomeSignUpButtonClicked()
    {
        binding.welcomeViewPager.currentItem = 1
    }
}