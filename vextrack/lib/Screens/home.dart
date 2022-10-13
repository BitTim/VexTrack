import 'package:flutter/material.dart';
import 'package:vextrack/Fragments/Forms/History/history_entry.dart';
import 'package:vextrack/Fragments/Home/contracts.dart';
import 'package:vextrack/Fragments/Home/history.dart';
import 'package:vextrack/Fragments/Home/home.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/screen_manager.dart';

import '../Services/auth.dart';

class _HomeState extends State<Home>
{
  late Function(int) _notifyParent;

	int _currentPage = 0;
  List<Widget> fragments = [];
  List<dynamic> keys = [];

  GlobalKey<FormState> historyFormKey = GlobalKey<FormState>();

  @override
  void initState()
  {
    keys.add(GlobalKey<HomeFragmentState>());
    keys.add(GlobalKey<ContractsFragmentState>());
    keys.add(GlobalKey<HistoryFragmentState>());

    fragments.add(HomeFragment(key: keys[0], uid: widget.uid));
    fragments.add(ContractsFragment(key: keys[1], uid: widget.uid));
    fragments.add(HistoryFragment(key: keys[2], uid: widget.uid, createUnlockedDialog: createUnlockedDialog));

    super.initState();
  }

  _HomeState(Function(int) notifyParent)
  {
    _notifyParent = notifyParent;
  }

  Widget? createUnlockedDialog({required List<String> unlocks, bool inverted = false})
  {
    if(unlocks.isEmpty) return null;

    return AlertDialog(
      scrollable: true,
      title: const Text("Unlocks"),
      
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.all(
          Radius.circular(16),
        ),
      ),
      content: Builder(
        builder: (context) {
          var width = MediaQuery.of(context).size.width;

          List<Widget> unlocksWidgets = [];
          for(String s in unlocks)
          {
            unlocksWidgets.add(Text("- $s"));
          }

          return SizedBox(
            width: width - 128, //TODO: Responsive layout (Fullscrenn diag on phones, restrained width on Desktop)
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: Row(
                mainAxisSize: MainAxisSize.max,
                children: [
                  Column(
                    children: [
                      if(!inverted) const Text("You have unlocked:"),
                      if(inverted) const Text("You no longer have unlocked:"),
                      ...unlocksWidgets,
                    ]
                  ),
                ],
              ),
            ),
          );
        },
      ),
      actions: [
        TextButton(
          onPressed: () {
            Navigator.of(context).pop();
          },
          child: const Text("OK"),
        ),
      ],
    );
  }

  Widget createHistoryDialog()
  {
    HistoryEntryForm form = HistoryEntryForm(formKey: historyFormKey);

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
            if(!historyFormKey.currentState!.validate()) return;

            Navigator.of(context).pop();
            List<String> unlocks = await DataService.addHistoryEntry(widget.uid, form.model);
            keys.elementAt(_currentPage).currentState!.update();

            Widget? unlockedDialog = createUnlockedDialog(unlocks: unlocks);
            if(unlockedDialog != null)
            {
              showDialog(
                context: context,
                builder: (context) {
                  return unlockedDialog;
                },
              );
            }
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
          if(_currentPage == 2) IconButton(
            icon: const Icon(Icons.filter_list_alt),
            onPressed: () {
              showDialog(
                context: context,
                builder: (BuildContext context) {
                  return keys.elementAt(_currentPage).currentState!.createFilterDialog();
                },
              );
            },
          ),
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

      body: fragments[_currentPage],

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

      floatingActionButton: FloatingActionButton(
        onPressed: () {
          showDialog(
            context: context,
            builder: (BuildContext context) {
              return createHistoryDialog();
            },
          );
        },
        child: const Icon(Icons.add),
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