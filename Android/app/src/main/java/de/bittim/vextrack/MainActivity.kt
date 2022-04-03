package de.bittim.vextrack

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.MenuItem
import androidx.fragment.app.Fragment
import androidx.viewpager.widget.ViewPager
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.tabs.TabLayoutMediator
import de.bittim.vextrack.databinding.ActivityMainBinding
import de.bittim.vextrack.fragments.*
import de.bittim.vextrack.fragments.adapters.ViewPagerAdapter

class MainActivity : AppCompatActivity()
{
    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?)
    {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initTabs(0, R.id.nav_dashboard)
    }

    private fun initTabs(initFrag: Int, initItemID: Int)
    {
        val adapter = ViewPagerAdapter(supportFragmentManager)

        adapter.addFragment(DashboardFragment(), "")
        adapter.addFragment(GoalsFragment(), "")
        adapter.addFragment(SeasonsFragment(), "")
        adapter.addFragment(HistoryFragment(), "")

        binding.viewPager.adapter = adapter

        // Add listener for bottom nav selection
        binding.bottomNav.setOnItemSelectedListener {
            // Update viewPager
            var selFrag: Int = 0
            when(it.itemId)
            {
                R.id.nav_dashboard -> selFrag = 0
                R.id.nav_goals -> selFrag = 1
                R.id.nav_seasons -> selFrag = 2
                R.id.nav_history -> selFrag = 3
            }

            binding.viewPager.setCurrentItem(selFrag)
            binding.headerTitle.text = it.title

            return@setOnItemSelectedListener true
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
                    0 -> selFrag = R.id.nav_dashboard
                    1 -> selFrag = R.id.nav_goals
                    2 -> selFrag = R.id.nav_seasons
                    3 -> selFrag = R.id.nav_history
                }

                binding.bottomNav.menu.findItem(selFrag).setChecked(true)
                binding.headerTitle.text = binding.bottomNav.menu.findItem(selFrag).title
            }
        })

        // Select fragment for initialization
        binding.viewPager.setCurrentItem(initFrag)
        binding.bottomNav.menu.findItem(initItemID).setChecked(true)
        binding.headerTitle.text = binding.bottomNav.menu.findItem(initItemID).title
    }
}