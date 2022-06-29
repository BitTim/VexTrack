import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/history_entry.dart';
import 'package:vextrack/Services/data.dart';

class HistoryEntryWidget extends StatefulWidget
{
  const HistoryEntryWidget({
    Key? key,
    required this.model,
  }) : super(key: key);

  final HistoryEntry model;

  @override
  HistoryEntryWidgetState createState() => HistoryEntryWidgetState();
}

class HistoryEntryWidgetState extends State<HistoryEntryWidget>
{
  @override
  Widget build(BuildContext context)
  {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      clipBehavior: Clip.antiAliasWithSaveLayer,
      child: SizedBox(
        height: 74,
        child: Stack(
          children: [
            Align(
              child: FutureBuilder<GameMap>(
                future: DataService.getMap(widget.model.map),
                builder: (context, snapshot) {
                  if (snapshot.hasError) {
                    return Text("Something went wrong! ${snapshot.error}");
                  }
                  else if (snapshot.hasData && snapshot.data != null) {
                    return FutureBuilder<String>(
                      future: DataService.getMapImgUrl(widget.model.map),
                      builder: (context, snapshot) {
                        if (snapshot.connectionState == ConnectionState.done)
                        {
                          return CachedNetworkImage(
                            imageUrl: snapshot.data.toString(),
                            width: MediaQuery.of(context).size.width,
                            height: MediaQuery.of(context).size.height * 1,
                            fit: BoxFit.cover,
                          );
                        }
                        else
                        {
                          return const SizedBox.shrink();
                        }
                      },
                    );
                  }
                  else
                  {
                    return const SizedBox.shrink();
                  }
                },
              ),
            ),
            Align(
              child: Container(
                decoration: const BoxDecoration(
                  color: Color(0x58000000),
                ),
              ),
            ),
            Align(
              child: Container(
                decoration: BoxDecoration(
                  gradient: widget.model.hasWon() ? AppColors.winGradient : widget.model.hasLost() ? AppColors.lossGradient : AppColors.drawGradient,
                ),
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Row(
                    mainAxisSize: MainAxisSize.max,
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Expanded(
                        child: Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                          child: Column(
                            mainAxisSize: MainAxisSize.max,
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                widget.model.getParsedDesc(),
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 24,
                                  fontWeight: FontWeight.w700,
                                  color: Colors.white,
                                ),
                              ),
                              Row(
                                mainAxisSize: MainAxisSize.max,
                                mainAxisAlignment: MainAxisAlignment.start,
                                crossAxisAlignment: CrossAxisAlignment.center,
                                children: [
                                  Padding(
                                    padding: const EdgeInsets.fromLTRB(0, 0, 8, 0),
                                    child: Text(
                                      "${widget.model.xp} XP",
                                      style: GoogleFonts.titilliumWeb(
                                        fontSize: 14,
                                        fontWeight: FontWeight.w500,
                                        color: Colors.white,
                                      ),
                                    ),
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.fromLTRB(8, 0, 0, 0),
                                    child: Text(
                                      widget.model.getParsedTime(),
                                      style: GoogleFonts.titilliumWeb(
                                        fontSize: 14,
                                        fontWeight: FontWeight.w500,
                                        color: Colors.white,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ),
                      Padding(
                        padding: const EdgeInsets.fromLTRB(4, 0, 0, 0),
                        child: Text(
                          widget.model.map.toUpperCase(),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 18,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        ),
                      ),
                    ]
                  ),
                ),
              ),
            )
          ],
        ),
      )
    );
  }
}