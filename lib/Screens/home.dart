import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:vextrack/screen_manager.dart';

import '../Services/auth.dart';

class _HomeState extends State
{
  late Function(int) _notifyParent;

	int _currentPage = 0;
	final List<bool> _epilogueState = [ false ];
  final List _children = [
		const Center(child: Text('Home'),),
		const Center(child: Text('Goals'),),
		const Center(child: Text('Seasons'),),
		const Center(child: Text('History'),),
	];

  _HomeState(Function(int) notifyParent)
  {
    _notifyParent = notifyParent;
  }

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
					PopupMenuButton(
						icon: const Icon(Icons.account_circle),
            onSelected: (value) {
              if(value == 'logout')
              {
                Provider.of<AuthService>(context, listen: false).signOut();
              }
              else if(value == 'settings')
              {
                _notifyParent(Screens.settings.index);
              }
            },
            itemBuilder: (BuildContext context) => [
              const PopupMenuItem(
                value: 'settings',
                child: Text('Settings'),
                ),
              const PopupMenuItem(
                value: 'logout',
                child: Text('Logout'),
              ),
            ],
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
  final Function(int) notifyParent;
	const Home({Key? key, required this.notifyParent}) : super(key: key);

	@override
	State createState()
	{
		// ignore: no_logic_in_create_state
		return _HomeState(notifyParent);
	}
}