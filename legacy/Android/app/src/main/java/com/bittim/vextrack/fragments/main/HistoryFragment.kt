package com.bittim.vextrack.fragments.main

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import com.bittim.vextrack.R
import com.bittim.vextrack.core.DataHandler
import com.bittim.vextrack.databinding.FragmentMainHistoryBinding
import com.google.firebase.firestore.DocumentSnapshot

class HistoryFragment : Fragment()
{
	override fun onCreate(savedInstanceState: Bundle?)
	{
		super.onCreate(savedInstanceState)
	}

	private var _binding: FragmentMainHistoryBinding? = null
	private val binding get() = _binding!!

	override fun onCreateView(
		inflater: LayoutInflater, container: ViewGroup?,
		savedInstanceState: Bundle?
	): View?
	{
		_binding = FragmentMainHistoryBinding.inflate(inflater, container, false)

		inflateHistory()
		DataHandler.subscribe(::updateData)

		return binding.root
	}

	override fun onDestroyView() {
		super.onDestroyView()
		_binding = null
	}

	fun updateData(snapshot: DocumentSnapshot?)
	{
		val a = snapshot?.data?.get("seasons")
		Log.d("HistoryFragment", a.toString())
	}

	fun inflateHistory() {
		// TODO: Fetch history objects

		// Create view
		val li = LayoutInflater.from(context).inflate(R.layout.component_history_element, null)
		binding.historyScrollViewContent.addView(li, binding.historyScrollViewContent.childCount)
		val v = binding.historyScrollViewContent.getChildAt(binding.historyScrollViewContent.childCount - 1)
		//v.elevation = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6f, resources.displayMetrics)

		// TMP: Using dummy data
		val title: TextView = v.findViewById(R.id.history_element_title)
		val xp: TextView = v.findViewById(R.id.history_element_xp)
		val map: TextView = v.findViewById(R.id.history_element_map)
		val time: TextView = v.findViewById(R.id.history_element_time)
		val banner: ImageView = v.findViewById(R.id.history_element_map_banner)

		title.text = "Competetive 13-5"
		xp.text = "5323 XP"
		map.text = "Split"
		time.text = "13:47"
		banner.setImageState(intArrayOf(R.attr.state_win, R.attr.state_split), true)
		banner.clipToOutline = true
	}
}