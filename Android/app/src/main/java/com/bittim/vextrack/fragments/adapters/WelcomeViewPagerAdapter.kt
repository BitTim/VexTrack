package com.bittim.vextrack.fragments.adapters

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentStatePagerAdapter

class WelcomeViewPagerAdapter(fm: FragmentManager) : FragmentStatePagerAdapter(fm, BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT)
{
    private val mFragmentList = ArrayList<Fragment>()

    override fun getItem(position: Int): Fragment
    {
        return mFragmentList[position]
    }

    override fun getCount(): Int
    {
        return mFragmentList.size
    }

    fun addItem(fragment: Fragment)
    {
        mFragmentList.add(fragment)
    }
}