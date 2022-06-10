import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../Services/auth_service.dart';

class _HomeState extends State
{
	int _currentPage = 0;
	final List<bool> _epilogueState = [ false ];
  final List _children = [
		const Center(child: Text('Home'),),
		const Center(child: Text('Goals'),),
		const Center(child: Text('Seasons'),),
		const Center(child: Text('History'),),
	];

	@override
	Widget build(BuildContext context) {
		return Scaffold(
				appBar: AppBar(
					title: const Text('VexTrack'),
				actions: [
					ToggleButtons(
						isSelected: _epilogueState,
						selectedBorderColor: Colors.blue,
						borderColor: Colors.grey,
						onPressed: (int index) {
							setState(() {
								_epilogueState[index] = !_epilogueState[index];
							});
						},
						children: const <Widget>[
							Icon(Icons.rocket_launch),
						],
					),
					IconButton(
						onPressed: () => {context.read<AuthService>().signOut()},
						icon: const Icon(Icons.account_circle)
					),
				],
				),

				body: _children[_currentPage],

				bottomNavigationBar: BottomNavigationBar(
					onTap: onTabTapped,
					currentIndex: _currentPage,
					unselectedItemColor: Colors.grey,
					selectedItemColor: Colors.blue,
					elevation: 8,
					items: const <BottomNavigationBarItem>[
						BottomNavigationBarItem(
							icon: Icon(Icons.home),
							label: 'Home',
						),
						BottomNavigationBarItem(
							icon: Icon(Icons.flag),
							label: 'Goals',
						),
						BottomNavigationBarItem(
							icon: Icon(Icons.cloud),
							label: 'Seasons',
						),
						BottomNavigationBarItem(
							icon: Icon(Icons.history),
							label: 'History',
						),
					],
				),
			);
	}

	void onTabTapped(int index) {
   setState(() {
     _currentPage = index;
   });
 }
}

class Home extends StatefulWidget
{
	const Home({Key? key}) : super(key: key);

	@override
	State createState()
	{
		return _HomeState();
	}
}