package com.bittim.vextrack

import android.content.Context
import android.content.Intent
import android.graphics.Rect
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.MenuInflater
import android.view.MotionEvent
import android.view.View
import android.view.inputmethod.InputMethodManager
import android.widget.EditText
import android.widget.PopupMenu
import android.widget.Toast
import androidx.viewpager.widget.ViewPager
import com.bittim.vextrack.databinding.ActivityMainBinding
import com.bittim.vextrack.fragments.main.GoalsFragment
import com.bittim.vextrack.fragments.main.HistoryFragment
import com.bittim.vextrack.fragments.main.HomeFragment
import com.bittim.vextrack.fragments.main.SeasonsFragment
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

    override fun dispatchTouchEvent(ev: MotionEvent?): Boolean
    {
        if (ev?.action == MotionEvent.ACTION_DOWN)
        {
            val v: View? = currentFocus
            if (v is EditText)
            {
                val outRect = Rect()
                v.getGlobalVisibleRect(outRect)
                if (!outRect.contains(ev.getRawX().toInt(), ev.getRawY().toInt()))
                {
                    v.clearFocus()
                    val imm: InputMethodManager = getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
                    imm.hideSoftInputFromWindow(v.windowToken, 0)
                }
            }
        }

        return super.dispatchTouchEvent(ev)
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
        val popup: PopupMenu = PopupMenu(this, binding.accountButton)
        popup.menuInflater.inflate(R.menu.account_popup, popup.menu)
        popup.show()

        popup.setOnMenuItemClickListener {
            when(it.itemId)
            {
                R.id.popup_settings -> {
                    onSettingsPopupClicked()
                    true
                }
                R.id.popup_sign_out -> {
                    onSignOutPopupClicked()
                    true
                }
                else -> false
            }
        }
    }

    private fun onSettingsPopupClicked()
    {
        startActivity(Intent(this@MainActivity, SettingsActivity::class.java))
    }

    private fun onSignOutPopupClicked()
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
}