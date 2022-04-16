package com.bittim.vextrack

import android.content.Context
import android.graphics.Rect
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.MotionEvent
import android.view.View
import android.view.inputmethod.InputMethodManager
import android.widget.EditText
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.viewpager.widget.ViewPager
import com.bittim.vextrack.databinding.ActivitySettingsBinding
import com.bittim.vextrack.fragments.adapters.ViewPagerAdapter
import com.bittim.vextrack.fragments.main.GoalsFragment
import com.bittim.vextrack.fragments.main.HistoryFragment
import com.bittim.vextrack.fragments.main.HomeFragment
import com.bittim.vextrack.fragments.main.SeasonsFragment
import com.bittim.vextrack.fragments.settings.AccountFragment
import com.bittim.vextrack.fragments.settings.AppearanceFragment
import com.bittim.vextrack.fragments.settings.GeneralFragment

class SettingsActivity : AppCompatActivity()
{
	private lateinit var binding: ActivitySettingsBinding

	private val generalFragment: GeneralFragment = GeneralFragment()
	private val appearanceFragment: AppearanceFragment = AppearanceFragment()
	private val accountFragment: AccountFragment = AccountFragment()

	override fun onCreate(savedInstanceState: Bundle?)
	{
		super.onCreate(savedInstanceState)
		binding = ActivitySettingsBinding.inflate(layoutInflater)
		setContentView(binding.root)

		initTabs()
		initButtons()
	}

	override fun dispatchTouchEvent(ev: MotionEvent?): Boolean
	{
		if (ev?.action == MotionEvent.ACTION_DOWN)
		{
			val v: View? = currentFocus
			if (v is  EditText)
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

		adapter.addItem(generalFragment, "")
		adapter.addItem(appearanceFragment, "")
		adapter.addItem(accountFragment, "")

		binding.viewPager.adapter = adapter
		binding.viewPager.currentItem = 0

		// Add listener for bottom nav selection
		binding.bottomNavigationView.setOnNavigationItemSelectedListener {
			// Update viewPager
			var selFrag: Int = 0
			when(it.itemId)
			{
				R.id.nav_general -> selFrag = 0
				R.id.nav_appearance -> selFrag = 1
				R.id.nav_account -> selFrag = 2
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
					0 -> selFrag = R.id.nav_general
					1 -> selFrag = R.id.nav_appearance
					2 -> selFrag = R.id.nav_account
				}

				binding.bottomNavigationView.menu.findItem(selFrag).setChecked(true)
				binding.title.text = binding.bottomNavigationView.menu.findItem(selFrag).title
			}
		})

		binding.bottomNavigationView.menu.findItem(R.id.nav_general).setChecked(true)
		binding.title.text = binding.bottomNavigationView.menu.findItem(R.id.nav_general).title
	}

	private fun initButtons()
	{
		binding.doneButton.setOnClickListener {
			accountFragment.pushProfile()
			finish()
		}
	}
}