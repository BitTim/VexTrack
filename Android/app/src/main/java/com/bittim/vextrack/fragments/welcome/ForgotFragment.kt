package com.bittim.vextrack.fragments.welcome

import android.content.Intent
import android.os.Bundle
import android.text.TextUtils
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import com.bittim.vextrack.MainActivity
import com.bittim.vextrack.R
import com.bittim.vextrack.WelcomeActivity
import com.bittim.vextrack.databinding.FragmentForgotBinding
import com.google.firebase.auth.FirebaseAuth

class ForgotFragment : Fragment() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
    }

    private var _binding: FragmentForgotBinding? = null
    private val binding get() = _binding!!

    private lateinit var auth: FirebaseAuth

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentForgotBinding.inflate(inflater, container, false)
        initButtons()

        auth = FirebaseAuth.getInstance()
        return binding.root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }



    // ================================
    //  Initializers
    // ================================

    private fun initButtons()
    {
        binding.forgotCancelButton.setOnClickListener { (activity as WelcomeActivity).changeFragmentLogIn(false) }
        binding.forgotResetButton.setOnClickListener { resetPassword() }
    }



    // ================================
    //  Utility
    // ================================

    private fun resetPassword()
    {
        val email: String = binding.forgotEmailEditText.text.toString()

        if (TextUtils.isEmpty(email))
        {
            binding.forgotEmailEditText.error = "E-Mail cannot be empty"
            binding.forgotEmailEditText.requestFocus()
            return
        }

        auth.sendPasswordResetEmail(email).addOnCompleteListener {
            if (it.isSuccessful)
            {
                (activity as WelcomeActivity).changeFragmentLogIn()
                Toast.makeText(activity as WelcomeActivity, "Recovery E-Mail sent to: $email. Make sure to check your spam folder", Toast.LENGTH_LONG).show()
            }
            else
            {
                Toast.makeText(activity as WelcomeActivity, "Error: " + it.exception?.message, Toast.LENGTH_LONG).show()
            }
        }
    }
}