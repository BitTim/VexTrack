package com.bittim.vextrack.core

import android.content.Context
import android.graphics.*
import android.net.Uri
import com.example.randomkolor.src.Format
import com.example.randomkolor.src.Luminosity
import com.example.randomkolor.src.RandomKolor
import java.io.ByteArrayOutputStream
import java.io.File
import java.io.FileOutputStream

class Utility
{
	companion object
	{
		fun genGenericProfilePic(context: Context, name: String?, width: Int = 320, height: Int = 320): Uri?
		{
			var uri: Uri? = null

			// Create Bitmap
			val bm: Bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888)
			val canvas: Canvas = Canvas(bm)

			// Setup colors
			val bgPaint = Paint()
			val fgPaint = Paint()

			bgPaint.color = Color.parseColor(RandomKolor().randomColor(luminosity = Luminosity.DARK, format = Format.HEX))
			fgPaint.color = Color.WHITE
			fgPaint.textSize = width.toFloat() / 2.5f

			// Draw Background
			canvas.drawRect(0f, 0f, width.toFloat(), height.toFloat(), bgPaint)

			// Draw Text
			val initials: String = name?.subSequence(0, 2) as String
			val bounds = Rect()

			fgPaint.getTextBounds(initials, 0, initials.length, bounds)
			val x: Float = (width - bounds.width()).toFloat() / 2
			val y: Float = (height + bounds.height()).toFloat() / 2

			canvas.drawText(initials, x, y, fgPaint)

			// Save as temporary file
			val outDir: File = context.cacheDir
			val outFile: File = File.createTempFile("genProfilePic", ".jpg", outDir)
			val fos = FileOutputStream(outFile)

			val stream = ByteArrayOutputStream()
			bm.compress(Bitmap.CompressFormat.JPEG, 100, stream)
			val data: ByteArray = stream.toByteArray()

			fos.write(data)
			fos.flush()
			fos.close()

			// Get URI from temp file
			uri = Uri.fromFile(outFile)

			return uri
		}
	}
}