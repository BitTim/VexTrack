import 'package:flutter/material.dart';
import 'package:vextrack/Components/FAB/action_button.dart';
import 'package:vextrack/Components/FAB/expandable_fab.dart';
import 'package:vextrack/Fragments/Forms/History/history_entry.dart';
import 'package:vextrack/Fragments/Home/contracts.dart';
import 'package:vextrack/Fragments/Home/history.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/screen_manager.dart';

import '../Services/auth.dart';

class _HomeState extends State<Home>
{
  late Function(int) _notifyParent;

	int _currentPage = 0;
  List<Widget> fragments = [];
  List<dynamic> keys = [];

  @override
  void initState()
  {
    keys.add(GlobalKey());
    keys.add(GlobalKey<ContractsFragmentState>());
    keys.add(GlobalKey<HistoryFragmentState>());

    fragments.add(const Center(child: Text("Home")));
    fragments.add(ContractsFragment(key: keys[1], uid: widget.uid));
    fragments.add(HistoryFragment(key: keys[2], uid: widget.uid));

    super.initState();
  }

  _HomeState(Function(int) notifyParent)
  {
    _notifyParent = notifyParent;
  }

  Widget createHistoryDialog()
  {
    HistoryEntryForm form = HistoryEntryForm();

    return AlertDialog(
      scrollable: true,
      title: const Text('Create history entry'),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.all(
          Radius.circular(16),
        ),
      ),
      content: Builder(
        builder: (context) {
          var width = MediaQuery.of(context).size.width;

          return SizedBox(
            width: width - 128, //TODO: Responsive layout (Fullscrenn diag on phones, restrained width on Desktop)
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: form,
            ),
          );
        },
      ),
      actions: [
        TextButton(
          onPressed: () {
            Navigator.of(context).pop();
          },
          child: const Text("Cancel"),
        ),
        ElevatedButton(
          onPressed: () async {
            Navigator.of(context).pop();
            await DataService.addHistoryEntry(widget.uid, form.model);
            if(_currentPage != 0) keys.elementAt(_currentPage).currentState!.update(); //FIXME: Temporary if statement
          },
          child: const Text("Create"),
        )
      ],
    );
  }

	@override
	Widget build(BuildContext context) {

		return Scaffold(
			appBar: AppBar(
				title: const Text('VexTrack'),
				actions: [
					PopupMenuButton(
						icon: const Icon(Icons.account_circle),
            onSelected: (value) {
              if(value == 'logout')
              {
                AuthService.signOut();
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

      body: fragments.elementAt(_currentPage),

      bottomNavigationBar: NavigationBar(
        labelBehavior: NavigationDestinationLabelBehavior.onlyShowSelected,
        onDestinationSelected: onTabTapped,
        selectedIndex: _currentPage,
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.home),
            label: 'Home',
          ),
          NavigationDestination(
            icon: Icon(Icons.flag),
            label: 'Contracts',
          ),
          NavigationDestination(
            icon: Icon(Icons.history),
            label: 'History',
          )
        ],
      ),

      floatingActionButton: ExpandableFab(
        distance: 64.0,
        children: [
          ActionButton(
            icon: const Icon(Icons.history_rounded),
            onPressed: () {
              showDialog(
                context: context,
                builder: (BuildContext context) {
                  return createHistoryDialog();
                },
              );
            },
          ),
          ActionButton(
            onPressed: () => ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Row(children: const [Icon(Icons.flag_rounded), Text("Create Contract")],))),
            icon: const Icon(Icons.flag_rounded),
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
  final String uid;
  final Function(int) notifyParent;

	const Home({Key? key, required this.uid, required this.notifyParent}) : super(key: key);

	@override
	State createState()
	{
		// ignore: no_logic_in_create_state
		return _HomeState(notifyParent);
	}
}