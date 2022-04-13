package com.bittim.vextrack

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentTransaction
import com.bittim.vextrack.databinding.ActivityWelcomeBinding
import com.bittim.vextrack.fragments.ForgotFragment
import com.bittim.vextrack.fragments.LogInFragment
import com.bittim.vextrack.fragments.SignUpFragment
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser

class WelcomeActivity : AppCompatActivity() {
    private lateinit var binding: ActivityWelcomeBinding
    private lateinit var auth: FirebaseAuth

    private var logInFragment: LogInFragment = LogInFragment()
    private var signUpFragment: SignUpFragment = SignUpFragment()
    private var forgotFragment: ForgotFragment = ForgotFragment()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityWelcomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        initPages()

        auth = FirebaseAuth.getInstance()
    }


    // ================================
    //  Initializers
    // ================================

    private fun initPages()
    {
        replaceFragment(logInFragment, noAnimation = true)
    }



    // ================================
    //  Utility
    // ================================

    public fun changeFragmentForgot() { replaceFragment(forgotFragment, true) }
    public fun changeFragmentSignUp() { replaceFragment(signUpFragment, false) }
    public fun changeFragmentLogIn(reversed: Boolean = false) { replaceFragment(logInFragment, reversed) }

    public fun startMainActivity()
    {
        startActivity(Intent(this, MainActivity::class.java))
        finish()
    }

    private fun replaceFragment(fragment: Fragment, reversed: Boolean = false, noAnimation: Boolean = false)
    {
        var enter = R.anim.fragment_enter_from_right
        var exit = R.anim.fragment_exit_to_left

        if (reversed)
        {
            enter = R.anim.fragment_enter_from_left
            exit = R.anim.fragment_exit_to_right
        }

        val ft: FragmentTransaction = supportFragmentManager.beginTransaction()
        if(!noAnimation) { ft.setCustomAnimations(enter, exit) }
        ft.replace(R.id.welcome_frameLayout, fragment)
        ft.commit()
    }
}