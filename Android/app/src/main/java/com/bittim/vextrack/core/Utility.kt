package com.bittim.vextrack.core

import android.graphics.Bitmap
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import com.example.randomkolor.src.Luminosity
import com.example.randomkolor.src.RandomKolor

class Utility
{
	public fun genGenericProfilePic(name: String)
	{
		val bm: Bitmap = Bitmap.createBitmap(100, 100, Bitmap.Config.ARGB_8888)
		val canvas: Canvas = Canvas(bm)

		val bgPaint: Paint = Paint()
		bgPaint.color = Color.parseColor((RandomKolor().randomColor(luminosity = Luminosity.LIGHT)))

		val fgPaint: Paint = Paint()
		fgPaint.setColor(Color.WHITE)

		canvas.drawRect(0f, 0f, 100f, 100f, bgPaint)

		// TODO: Implement rendering of Text
		val letter: String = name.elementAt(0).toString()
		// canvas.drawText(letter, )
	}
}