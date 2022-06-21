import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:vextrack/Models/season.dart';
import 'package:vextrack/Services/data.dart';

class HomeFragment extends StatelessWidget
{
  const HomeFragment({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return StreamBuilder<List<Season>>(
      stream: context.read<DataService>().seasons,
      builder: (context, snapshot) {
        if (snapshot.hasError) {
          return Text("Something went wrong! ${snapshot.error}");
        }
        else if (snapshot.hasData) 
        {
          final seasons = snapshot.data!;

          return ListView(
            children: seasons.map((season) => ListTile(
              title: Text(season.name),
              subtitle: Text("Active Level: ${season.activeLevel}"),
              trailing: Text("Active XP: ${season.activeXP}"),
            )).toList()
          );
        }
        else
        {
          return const Center(child: CircularProgressIndicator());
        }
      },
    );
  }
}