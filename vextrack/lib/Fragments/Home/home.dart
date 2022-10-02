import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/daily_contract.dart';
import 'package:vextrack/Components/dashboard_chart.dart';
import 'package:vextrack/Models/Contracts/daily_contract.dart';
import 'package:vextrack/Services/data.dart';

class HomeFragment extends StatefulWidget
{
  final String uid;
  const HomeFragment({super.key, required this.uid});

  @override
  HomeFragmentState createState() => HomeFragmentState();
}

class HomeFragmentState extends State<HomeFragment>
{
  DailyContract? dailyContract;
  String name = "";
  late Future<bool> updateFuture;

  @override
  void initState() {
    super.initState();
    updateFuture = update();
  }

  Future<bool> update() async
  {
    await DataService.getActiveSeasonMeta(widget.uid);
    if(DataService.userData == null) await DataService.fetchUserData(widget.uid);
    DailyContract dc = await DailyContract.init(widget.uid);

    setState(() {
      name = DataService.userData!.name;
      dailyContract = dc;
    });

    return true;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: updateFuture,
      builder: (context, AsyncSnapshot<void> snapshot) {
        if(snapshot.hasData)
        {
          return Padding(
            padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
            child: SingleChildScrollView(
              child: Column(
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                    child: Row(
                      children: [
                        Text(
                          "Welcome back $name",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ],
                    ),
                  ),
                  if(dailyContract != null) DailyContractWidget(
                    model: dailyContract!,
                  ),
                  DashboardChart(uid: widget.uid),
                  const SizedBox(
                    height: 88,
                  ),
                ],
              ),
            ),
          );
        } else {
          return const Center(child: CircularProgressIndicator());
        }
      }
    );
  }
}