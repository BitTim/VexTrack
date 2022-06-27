import 'package:cached_network_image/cached_network_image.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/history_entry.dart';
import 'package:vextrack/Services/data.dart';

class HistoryEntryWidget extends StatefulWidget
{
  const HistoryEntryWidget({
    Key? key,
    this.desc = "",
    this.xp = 0,
    this.time = "00:00 01.01.1970",
    this.map = "None",
    this.surrendered = false,
  }) : super(key: key);

  final String desc;
  final int xp;
  final String time;
  final String map;
  final bool surrendered;

  @override
  HistoryEntryWidgetState createState() => HistoryEntryWidgetState();

  static HistoryEntryWidget fromModel(HistoryEntry he)
  {
    String desc = he.desc != "" ? he.desc : "${he.mode} ${he.score}-${he.enemyScore}";
    int xp = he.xp;
    String time = DateTime.fromMillisecondsSinceEpoch(he.time * 1000).toLocal().toString();
    String map = he.map.toLowerCase();
    bool surrendered = he.surrenderedLoss || he.surrenderedWin;

    final heWidget = HistoryEntryWidget(
      desc: desc,
      xp: xp,
      time: time,
      map: map,
      surrendered: surrendered,
    );

    return heWidget;
  }
}

class HistoryEntryWidgetState extends State<HistoryEntryWidget>
{
  @override
  Widget build(BuildContext context)
  {
    return Card(
      child: Stack(
        children: [
          FutureBuilder<GameMap>(
            future: DataService.getMap(widget.map),
            builder: (context, snapshot) {
              if (snapshot.hasError) {
                return Text("Something went wrong! ${snapshot.error}");
              }
              else if (snapshot.hasData && snapshot.data != null) {
                return FutureBuilder<String>(
                  future: DataService.getMapImgUrl(widget.map),
                  builder: (context, snapshot) {
                    if (snapshot.connectionState == ConnectionState.done)
                    {
                      return Image.network(snapshot.data.toString());
                    }
                    else
                    {
                      return const Center(child: CircularProgressIndicator());
                    }
                  },
                );
              }
              else
              {
                return const Center(child: CircularProgressIndicator());
              }
            },
          ),
          Container(
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      children: [
                        Text(widget.desc),
                        Row(
                          children: [
                            Text("${widget.xp} XP"),
                            Text(widget.time),
                          ],
                        ),
                      ],
                    ),
                  ),
                  Text(widget.map.toUpperCase()),
                ]
              ),
            ),
          )
        ],
      )
    );
  }
}