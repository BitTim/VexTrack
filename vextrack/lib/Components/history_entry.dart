import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Models/History/history_entry.dart';
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
  Gradient getGradient()
  {
    if (widget.model.hasWon()) return AppColors.winToTransparentGradient;
    if (widget.model.hasLost()) return AppColors.lossToTransparentGradient;

    return AppColors.drawToTransparentGradient;
  }

  @override
  Widget build(BuildContext context)
  {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 4, 0, 0),
      child: Card(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        surfaceTintColor: Theme.of(context).colorScheme.surface,
        clipBehavior: Clip.antiAliasWithSaveLayer,
        elevation: 8,
        child: SizedBox(
          height: 74,
          child: Stack(
            children: [
              Align(
                child: FutureBuilder<String>(
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
                    gradient: getGradient(),
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
                              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  widget.model.getFormattedDesc(),
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
                                        widget.model.getFormattedTime(),
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
                                        widget.model.getFormattedXP(),
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
                          child: Column(
                            mainAxisSize: MainAxisSize.max,
                            mainAxisAlignment: MainAxisAlignment.end,
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              if(widget.model.hasSurrendered()) const Padding(
                                padding: EdgeInsets.fromLTRB(0, 0, 0, 4),
                                child: Icon(
                                  Icons.flag_rounded,
                                  color: Colors.white,
                                ),
                              ),
                              Text(
                                DataService.getMapName(widget.model.map),
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 18,
                                  fontWeight: FontWeight.w700,
                                  color: Colors.white,
                                ),
                              ),
                            ],
                          ),
                        ),
                        Align(
                          alignment: Alignment.centerRight,
                          child: Padding(
                            padding: const EdgeInsets.fromLTRB(8, 0, 0, 0),
                            child: PopupMenuButton(
                              icon: const Icon(
                                Icons.more_vert,
                                color: Colors.white,
                              ),
                              onSelected: (value) {
                                if(value == 'edit') {
                                  //TODO: Add functionality
                                }
                                else if(value == 'delete') {
                                  //TODO: Add functionality
                                }
                              },
                              itemBuilder: (BuildContext context) => [
                                const PopupMenuItem(
                                  value: 'edit',
                                  child: Text('Edit'),
                                  ),
                                PopupMenuItem(
                                  value: 'delete',
                                  child: Text(
                                    'Delete',
                                    style: GoogleFonts.titilliumWeb(
                                      color: AppColors.loss[0],
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        )
                      ]
                    ),
                  ),
                ),
              )
            ],
          ),
        )
      ),
    );
  }
}