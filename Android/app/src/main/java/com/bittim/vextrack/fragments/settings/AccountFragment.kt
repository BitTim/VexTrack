package com.bittim.vextrack.fragments.settings

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.activity.result.contract.ActivityResultContracts
import com.bittim.vextrack.SettingsActivity
import com.bittim.vextrack.databinding.FragmentSettingsAccountBinding
import com.canhub.cropper.CropImage
import com.canhub.cropper.CropImageContract
import com.canhub.cropper.CropImageView
import com.canhub.cropper.options
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.auth.UserProfileChangeRequest
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.tasks.await
import kotlinx.coroutines.withContext

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
		getProfile()

		initButtons()
		
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
		binding.settingsProfilePictureImageButton.setOnClickListener { onProfilePicButtonClicked() }
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



	// ================================
	//  Utility
	// ================================

	private val getCroppedResult = registerForActivityResult(CropImageContract())
	{
		if (it.isSuccessful)
		{
			photoURI = it.originalUri
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
	}

	public fun pushProfile()
	{
		auth.currentUser?.let { user ->
			val username = binding.settingsUsernameEditText.text.toString()
			val profileUpdates = UserProfileChangeRequest.Builder()
				.setDisplayName(username)
				.setPhotoUri(photoURI)
				.build()

			CoroutineScope(Dispatchers.IO).launch {
				try
				{
					user.updateProfile(profileUpdates).await()
					withContext(Dispatchers.Main)
					{
						Toast.makeText(activity as SettingsActivity, "Successfully updated Profile", Toast.LENGTH_LONG).show()
					}
				} catch (e: Exception) {
					withContext(Dispatchers.Main)
					{
						Toast.makeText(activity as SettingsActivity, e.message, Toast.LENGTH_LONG).show()
					}
				}
			}
		}
	}
}