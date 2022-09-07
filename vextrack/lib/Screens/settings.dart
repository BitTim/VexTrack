import 'package:flutter/material.dart';
import 'package:vextrack/screen_manager.dart';

class _SettingsState extends State {
  late Function(int) _notifyParent;

  int _currentPage = 0;
  final List _children = [
		const Center(child: Text('General'),),
		const Center(child: Text('Appearance'),),
		const Center(child: Text('Account'),),
	];

  _SettingsState(Function(int) notifyParent)
  {
    _notifyParent = notifyParent;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('VexTrack Settings'),
        actions: [
          IconButton(
            onPressed: () {
              _notifyParent(Screens.home.index);
            },
            icon: const Icon(Icons.check)
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
            icon: Icon(Icons.settings),
            label: 'General',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.contrast),
            label: 'Appearance',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.account_circle),
            label: 'Account',
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

class Settings extends StatefulWidget
{
  final String uid;
  final Function(int) notifyParent;

  const Settings({Key? key, required this.uid, required this.notifyParent}) : super(key: key);

  @override
  State createState()
  {
    // ignore: no_logic_in_create_state
    return _SettingsState(notifyParent);
  }
}