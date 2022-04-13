package com.bittim.vextrack

import android.content.Intent
import android.net.Uri
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import androidx.viewpager.widget.ViewPager
import com.bittim.vextrack.databinding.ActivityMainBinding
import com.bittim.vextrack.fragments.GoalsFragment
import com.bittim.vextrack.fragments.HistoryFragment
import com.bittim.vextrack.fragments.HomeFragment
import com.bittim.vextrack.fragments.SeasonsFragment
import com.bittim.vextrack.fragments.adapters.ViewPagerAdapter
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.auth.UserProfileChangeRequest
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.tasks.await
import kotlinx.coroutines.withContext

class MainActivity : AppCompatActivity() {
    private lateinit var binding: ActivityMainBinding
    private lateinit var auth: FirebaseAuth

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        auth = FirebaseAuth.getInstance()

        initTabs()
        initButtons()
    }

    override fun onStart()
    {
        super.onStart()
        checkUser()
    }



    // ================================
    //  Initializers
    // ================================

    private fun initTabs()
    {
        val adapter = ViewPagerAdapter(supportFragmentManager)

        adapter.addItem(HomeFragment(), "")
        adapter.addItem(GoalsFragment(), "")
        adapter.addItem(SeasonsFragment(), "")
        adapter.addItem(HistoryFragment(), "")

        binding.viewPager.adapter = adapter
        binding.viewPager.currentItem = 0

        // Add listener for bottom nav selection
        binding.bottomNavigationView.setOnNavigationItemSelectedListener {
            // Update viewPager
            var selFrag: Int = 0
            when(it.itemId)
            {
                R.id.nav_home -> selFrag = 0
                R.id.nav_goals -> selFrag = 1
                R.id.nav_seasons -> selFrag = 2
                R.id.nav_history -> selFrag = 3
            }

            binding.viewPager.currentItem = selFrag
            binding.title.text = it.title

            return@setOnNavigationItemSelectedListener true
        }

        // Add listener for page changes
        binding.viewPager.addOnPageChangeListener(object: ViewPager.OnPageChangeListener
        {
            override fun onPageScrollStateChanged(state: Int) { }

            override fun onPageScrolled(position: Int, positionOffset: Float, positionOffsetPixels: Int) { }

            override fun onPageSelected(position: Int)
            {
                // Update BottomNavigationView
                var selFrag: Int = 0
                when(position)
                {
                    0 -> selFrag = R.id.nav_home
                    1 -> selFrag = R.id.nav_goals
                    2 -> selFrag = R.id.nav_seasons
                    3 -> selFrag = R.id.nav_history
                }

                binding.bottomNavigationView.menu.findItem(selFrag).setChecked(true)
                binding.title.text = binding.bottomNavigationView.menu.findItem(selFrag).title
            }
        })

        binding.bottomNavigationView.menu.findItem(R.id.nav_home).setChecked(true)
        binding.title.text = binding.bottomNavigationView.menu.findItem(R.id.nav_home).title
    }

    private fun initButtons()
    {
        binding.epilogueButton.setOnClickListener { onEpilogueButtonClicked() }
        binding.accountButton.setOnClickListener { onAccountButtonClicked() }
    }



    // ================================
    //  Button Handlers
    // ================================

    private fun onEpilogueButtonClicked()
    {
        binding.epilogueButton.isActivated = !binding.epilogueButton.isActivated
    }

    private fun onAccountButtonClicked()
    {
        auth.signOut()
        checkUser()
    }



    // ================================
    //  Utility
    // ================================

    private fun checkUser()
    {
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

        binding.accountButton.setImageURI(user?.photoUrl)
        binding.accountButton.clipToOutline = true
    }







    // TODO: Move to Settings Fragment
    private fun updateProfile()
    {
        auth.currentUser?.let { user ->
            //val username = binding.settings_username_editText
            //val photoURI = Uri.parse("android.resource://$packageName/${R.drawable.helddersteine}")
            val profileUpdates = UserProfileChangeRequest.Builder()
                //.setDisplayName(username)
                //.setPhotoUri(photoURI)
                .build()

            CoroutineScope(Dispatchers.IO).launch {
                try
                {
                	user.updateProfile(profileUpdates).await()
                    withContext(Dispatchers.Main)
                    {
                        Toast.makeText(this@MainActivity, "Successfully updated Profile", Toast.LENGTH_LONG).show()
                    }
                } catch (e: Exception) {
                    withContext(Dispatchers.Main)
                    {
                        Toast.makeText(this@MainActivity, e.message, Toast.LENGTH_LONG).show()
                    }
                }
            }
        }
    }
}