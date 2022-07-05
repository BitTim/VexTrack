import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:percent_indicator/linear_percent_indicator.dart';
import 'package:vextrack/Models/season.dart';
import 'package:vextrack/Services/data.dart';

class SeasonWidget extends StatefulWidget {
  const SeasonWidget({
    Key? key,
    required this.model,
  }) : super(key: key);

  final Season model;

  @override
  SeasonWidgetState createState() => SeasonWidgetState();
}

class SeasonWidgetState extends State<SeasonWidget>
{
  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      clipBehavior: Clip.antiAliasWithSaveLayer,
      child: SizedBox(
        height: 96,
        child: Stack(
          children: [
            Align(
              child: FutureBuilder<String>(
                future: DataService.getSeasonImgUrl(widget.model.id),
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
              child: Column(
                mainAxisSize: MainAxisSize.max,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                    child: Row(
                      mainAxisSize: MainAxisSize.max,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          DataService.getSeasonName(widget.model.id),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        ),
                        const Icon(
                          Icons.check, color: Colors.green
                        ), //TODO: Change icons depending on state
                      ],
                    ),
                  ),
                  Padding( // TODO: Rework Progress Bars //TODO: Progress bars should match icon color
                    padding: const EdgeInsets.fromLTRB(0, 4, 8, 0),
                    child: Row(
                      mainAxisSize: MainAxisSize.max,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                          child: LinearPercentIndicator(
                            width: MediaQuery.of(context).size.width * 0.6,
                            percent: 0.5,
                            lineHeight: 8,
                            animation: true,
                            barRadius: const Radius.circular(4),
                            trailing: LinearPercentIndicator(
                              width: MediaQuery.of(context).size.width * 0.2,
                              percent: 0,
                              lineHeight: 8,
                              animation: true,
                              barRadius: const Radius.circular(4),
                            ),
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.fromLTRB(4, 0, 0, 0),
                          child: Text(
                            '50%',
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 4),
                    child: Text(
                      "${DataService.getSeasonFormattedStartDate(widget.model.id)} - ${DataService.getSeasonFormattedEndDate(widget.model.id)}",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: Colors.white,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}