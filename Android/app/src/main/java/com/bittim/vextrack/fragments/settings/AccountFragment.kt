package com.bittim.vextrack.fragments.settings

import android.app.AlertDialog
import android.content.DialogInterface
import android.net.Uri
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import com.bittim.vextrack.R
import com.bittim.vextrack.SettingsActivity
import com.bittim.vextrack.core.Utility
import com.bittim.vextrack.databinding.FragmentSettingsAccountBinding
import com.canhub.cropper.CropImageContract
import com.canhub.cropper.CropImageView
import com.canhub.cropper.options
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser

class AccountFragment : Fragment()
{
	override fun onCreate(savedInstanceState: Bundle?)
	{
		super.onCreate(savedInstanceState)
	}

	private var _binding: FragmentSettingsAccountBinding? = null
	private val binding get() = _binding!!

	private lateinit var auth: FirebaseAuth
	private var photoURI: Uri? = null

	override fun onCreateView(
		inflater: LayoutInflater, container: ViewGroup?,
		savedInstanceState: Bundle?
	): View
	{
		_binding = FragmentSettingsAccountBinding.inflate(inflater, container, false)

		auth = FirebaseAuth.getInstance()
		initButtons()
		
		return binding.root
	}

	override fun onStart()
	{
		super.onStart()
		getProfile()
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
		binding.settingsProfilePictureImageButton.setOnClickListener { onProfilePicButtonClicked() }
		binding.settingsResetPicButton.setOnClickListener { onResetPicClicked() }
	}



	// ================================
	//  Button Handlers
	// ================================

	private fun onProfilePicButtonClicked()
	{
		getCroppedResult.launch(
			options {
				setGuidelines(CropImageView.Guidelines.OFF)
				setAspectRatio(1, 1)
				setFixAspectRatio(true)
				setImageSource(includeGallery = true, includeCamera = false)
			}
		)
	}

	private fun onResetPicClicked()
	{
		val dialogBuilder = AlertDialog.Builder(activity)
		dialogBuilder.setTitle(getString(R.string.settings_reset_pic_dialog_title))
		dialogBuilder.setMessage(getString(R.string.settings_reset_pic_dialog_message))

		dialogBuilder.setNegativeButton(getString(R.string.no), DialogInterface.OnClickListener { dialog, _ ->
			// No confirmation
			dialog.cancel()
		})

		dialogBuilder.setPositiveButton(getString(R.string.yes), DialogInterface.OnClickListener { dialog, _ ->
			// Yes confirmation
			photoURI = Utility.genGenericProfilePic(activity as SettingsActivity, auth.currentUser?.displayName)
			updateProfilePicture()
			dialog.cancel()
		})

		val dialog = dialogBuilder.create()
		dialog.setOnShowListener {
			dialog.getButton(AlertDialog.BUTTON_POSITIVE).setTextColor(ContextCompat.getColor(activity as SettingsActivity, R.color.red1))
		}

		dialog.show()
	}



	// ================================
	//  Utility
	// ================================

	private val getCroppedResult = registerForActivityResult(CropImageContract())
	{
		if (it.isSuccessful)
		{
			photoURI = it.uriContent
			updateProfilePicture()
		}
	}

	private fun updateProfilePicture()
	{
		binding.settingsProfilePictureImageButton.setImageURI(photoURI)
		binding.settingsProfilePictureImageButton.clipToOutline = true
	}

	private fun getProfile()
	{
		val user: FirebaseUser? = auth.currentUser

		photoURI = user?.photoUrl
		binding.settingsUsernameEditText.setText(user?.displayName)

		updateProfilePicture()
	}

	fun getProfileData(): Triple<String, String, String>
	{
		val username = binding.settingsUsernameEditText.text.toString()
		val email = binding.settingsEmailEditText.text.toString()
		return Triple(username, email, photoURI.toString())
	}
}