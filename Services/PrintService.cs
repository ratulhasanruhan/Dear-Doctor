using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Dear_Doctor.Models;

namespace Dear_Doctor.Services
{
    public class PrintService
    {
        // 96 DPI dimensions for A4 (21 cm x 29.7 cm)
        private const double A4Width = 794;
        private const double A4Height = 1123;
        private const double TopMarginHeight = 189; // 5 cm = 50mm = ~189 pixels
        private const double LeftColumnWidth = 208;  // 5.5 cm = 55mm = ~208 pixels
        private const double LateralMargin = 56;     // 1.5 cm = 15mm = ~56 pixels

        public static void Print(Prescription prescription)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Create programmatic root element for the printout
                Border pageBorder = new Border
                {
                    Width = A4Width,
                    Height = A4Height,
                    Background = Brushes.White,
                    Padding = new Thickness(LateralMargin, 0, LateralMargin, LateralMargin)
                };

                // Root layout grid
                Grid mainGrid = new Grid();
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(TopMarginHeight) }); // Row 0: 5cm blank space
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Row 1: Patient details
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Row 2: Rx Content
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Row 3: Footer/Signature

                // --- ROW 0: Blank space ---
                Border blankHeader = new Border { Background = Brushes.Transparent };
                Grid.SetRow(blankHeader, 0);
                mainGrid.Children.Add(blankHeader);

                // --- ROW 1: Patient Details ---
                Grid patientGrid = new Grid { Margin = new Thickness(0, 10, 0, 10) };
                patientGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Top line
                patientGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Details
                patientGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Bottom line

                // Top Line
                // Border topLine = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 1, 0, 0), Height = 1, Margin = new Thickness(0, 0, 0, 8) };
                // Grid.SetRow(topLine, 0);
                // patientGrid.Children.Add(topLine);

                // Patient Info Columns
                Grid infoColumns = new Grid();
                infoColumns.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Name
                infoColumns.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Age
                infoColumns.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Gender
                infoColumns.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Date

                // Helper to add TextBlock labels
                var addPatientField = new Action<string, string, int, double>((label, val, col, rightMargin) =>
                {
                    StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, rightMargin, 0) };
                    sp.Children.Add(new TextBlock { Text = label, FontWeight = FontWeights.Bold, FontSize = 14, Foreground = Brushes.Black, FontFamily = new FontFamily("Segoe UI") });
                    sp.Children.Add(new TextBlock { Text = val, FontSize = 14, Foreground = Brushes.Black, FontFamily = new FontFamily("Segoe UI") });
                    Grid.SetColumn(sp, col);
                    infoColumns.Children.Add(sp);
                });

                addPatientField("Name: ", prescription.PatientName, 0, 0);
                addPatientField("Age: ", prescription.Age, 1, 24);
                addPatientField("Gender: ", prescription.Gender, 2, 24);
                addPatientField("Date: ", prescription.Date.ToString("dd-MM-yyyy"), 3, 0);

                Grid.SetRow(infoColumns, 1);
                patientGrid.Children.Add(infoColumns);

                // Bottom Line
                Border bottomLine = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 1, 0, 0), Height = 1, Margin = new Thickness(0, 8, 0, 0) };
                Grid.SetRow(bottomLine, 2);
                patientGrid.Children.Add(bottomLine);

                Grid.SetRow(patientGrid, 1);
                mainGrid.Children.Add(patientGrid);

                // --- ROW 2: Main Prescription Columns Grid ---
                Grid bodyGrid = new Grid { Margin = new Thickness(0, 10, 0, 0) };
                bodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(LeftColumnWidth) }); // Col 0: Left Column (5.5 cm)
                bodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5) }); // Col 1: Vertical Divider Line
                bodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Col 2: Rx Column

                // Left Column content (vitals + clinical info)
                StackPanel leftPanel = new StackPanel { Margin = new Thickness(0, 0, 12, 0) };
                var addClinicalBlock = new Action<string, string>((header, content) =>
                {
                    StackPanel sp = new StackPanel { Margin = new Thickness(0, 0, 0, 16) };
                    sp.Children.Add(new TextBlock { Text = header, FontWeight = FontWeights.Bold, FontSize = 12, Foreground = Brushes.Black, Margin = new Thickness(0, 0, 0, 4), FontFamily = new FontFamily("Segoe UI") });
                    sp.Children.Add(new TextBlock { Text = content, FontSize = 13, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap, FontFamily = new FontFamily("Segoe UI") });
                    leftPanel.Children.Add(sp);
                });

                // Vitals
                StackPanel vitalsPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 16) };
                vitalsPanel.Children.Add(new TextBlock { Text = "B.P: ", FontWeight = FontWeights.Bold, FontSize = 12, Foreground = Brushes.Black, Margin = new Thickness(0, 0, 0, 2), FontFamily = new FontFamily("Segoe UI") });
                vitalsPanel.Children.Add(new TextBlock { Text = string.IsNullOrWhiteSpace(prescription.BP) ? "" : prescription.BP + " mmHg", FontSize = 13, Foreground = Brushes.Black, Margin = new Thickness(0, 0, 0, 10), FontFamily = new FontFamily("Segoe UI") });
                vitalsPanel.Children.Add(new TextBlock { Text = "P.R: ", FontWeight = FontWeights.Bold, FontSize = 12, Foreground = Brushes.Black, Margin = new Thickness(0, 0, 0, 2), FontFamily = new FontFamily("Segoe UI") });
                vitalsPanel.Children.Add(new TextBlock { Text = prescription.PR, FontSize = 13, Foreground = Brushes.Black, FontFamily = new FontFamily("Segoe UI") });
                leftPanel.Children.Add(vitalsPanel);

                addClinicalBlock("C/O:", prescription.ChiefComplaints);
                addClinicalBlock("Most probable diagnosis:", prescription.Diagnosis);
                addClinicalBlock("Advice:", prescription.Advice);
                addClinicalBlock("Complications:", prescription.Complication);

                Grid.SetColumn(leftPanel, 0);
                bodyGrid.Children.Add(leftPanel);

                // Vertical Divider Line
                Border verticalDivider = new Border { Background = Brushes.Black, Width = 1.5, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(0, -10, 0, 0) };
                Grid.SetColumn(verticalDivider, 1);
                bodyGrid.Children.Add(verticalDivider);

                // Right Column content (Rx Medicines)
                Grid rxGrid = new Grid { Margin = new Thickness(16, 0, 0, 0) };
                rxGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Rx Symbol
                rxGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Grid Items

                // Rx Symbol
                TextBlock rxSymbol = new TextBlock { Text = "Rx,", FontSize = 36, FontWeight = FontWeights.ExtraBold, FontFamily = new FontFamily("Times New Roman"), Foreground = Brushes.Black, Margin = new Thickness(0, 0, 0, 10) };
                Grid.SetRow(rxSymbol, 0);
                rxGrid.Children.Add(rxSymbol);

                // Medicine List items container
                StackPanel medicinesList = new StackPanel();

                // Helper to create a bordered, formatted table for a category
                var CreateTable = new Func<string, List<PrescribedItem>, FrameworkElement>((categoryTitle, items) =>
                {
                    StackPanel panel = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

                    // 1. Category Title
                    /*
                    panel.Children.Add(new TextBlock
                    {
                        Text = categoryTitle,
                        FontWeight = FontWeights.Bold,
                        FontSize = 12,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(0, 0, 0, 8),
                        TextDecorations = TextDecorations.Underline,
                        FontFamily = new FontFamily("Segoe UI")
                    });
                    */

                    // 2. Table Border (Outer Box)
                    Border tableBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    Grid tableGrid = new Grid();
                    tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Row 0: Header
                    tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Row 1: Rows list

                    // Table Header Grid
                    Grid header = new Grid { Background = new SolidColorBrush(Color.FromRgb(243, 244, 246)) };
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                    var createHeaderCell = new Action<string, int, bool>((text, col, hasRightBorder) =>
                    {
                        Border b = new Border
                        {
                            Padding = new Thickness(col == 0 ? 4 : 8, 6, 8, 6),
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(0, 0, hasRightBorder ? 1 : 0, 1)
                        };
                        b.Child = new TextBlock
                        {
                            Text = text,
                            FontWeight = FontWeights.Bold,
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            FontFamily = new FontFamily("Segoe UI"),
                            HorizontalAlignment = col == 0 ? HorizontalAlignment.Center : HorizontalAlignment.Left
                        };
                        Grid.SetColumn(b, col);
                        header.Children.Add(b);
                    });

                    createHeaderCell("SL", 0, true);
                    createHeaderCell("Medicine and Composition", 1, true);
                    createHeaderCell("Dosage", 2, true);
                    
                    createHeaderCell("Duration", 3, false);

                    Grid.SetRow(header, 0);
                    tableGrid.Children.Add(header);

                    // Table Rows
                    StackPanel rowsPanel = new StackPanel();
                    int idx = 1;
                    foreach (var item in items)
                    {
                        // Outer Row Border (bottom divider + alternating background)
                        Border rowBorder = new Border
                        {
                            BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235)), // #E5E7EB
                            BorderThickness = new Thickness(0, 0, 0, 1),
                            Background = (idx % 2 == 0) ? new SolidColorBrush(Color.FromRgb(245, 245, 245)) : Brushes.Transparent
                        };

                        Grid row = new Grid { MinHeight = 30 };
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
 
                        // Cell 0: SL
                        Border slBorder = new Border
                        {
                            BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235)),
                            BorderThickness = new Thickness(0, 0, 1, 0),
                            Padding = new Thickness(4)
                        };
                        slBorder.Child = new TextBlock
                        {
                            Text = idx.ToString(),
                            FontWeight = FontWeights.Bold,
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = new FontFamily("Segoe UI")
                        };
                        Grid.SetColumn(slBorder, 0);
                        row.Children.Add(slBorder);
 
                        // Cell 1: Name and Composition
                        Border nameBorder = new Border
                        {
                            BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235)),
                            BorderThickness = new Thickness(0, 0, 1, 0),
                            Padding = new Thickness(8, 4, 8, 4)
                        };
                        StackPanel nameStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                        nameStack.Children.Add(new TextBlock
                        {
                            Text = item.MedicineName,
                            FontWeight = FontWeights.SemiBold,
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            TextWrapping = TextWrapping.Wrap,
                            FontFamily = new FontFamily("Segoe UI")
                        });
                        if (!string.IsNullOrWhiteSpace(item.GenericName))
                        {
                            nameStack.Children.Add(new TextBlock
                            {
                                 Text = item.GenericName,
                                 FontSize = 10,
                                 Foreground = Brushes.DarkGray,
                                 Margin = new Thickness(0, 2, 0, 0),
                                 TextWrapping = TextWrapping.Wrap,
                                 FontStyle = FontStyles.Italic,
                                 FontFamily = new FontFamily("Segoe UI")
                            });
                        }
                        nameBorder.Child = nameStack;
                        Grid.SetColumn(nameBorder, 1);
                        row.Children.Add(nameBorder);
 
                        // Cell 2: Dosage
                        Border doseBorder = new Border
                        {
                            BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235)),
                            BorderThickness = new Thickness(0, 0, 1, 0),
                            Padding = new Thickness(8, 4, 8, 4)
                        };
                        doseBorder.Child = new TextBlock
                        {
                            Text = item.DoseAndInstructions,
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            FontFamily = new FontFamily("Segoe UI")
                        };
                        Grid.SetColumn(doseBorder, 2);
                        row.Children.Add(doseBorder);
 

 
                        // Cell 4: Duration
                        Border durationBorder = new Border
                        {
                            Padding = new Thickness(8, 4, 8, 4)
                        };
                        durationBorder.Child = new TextBlock
                        {
                            Text = item.Duration,
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = new FontFamily("Segoe UI")
                        };
                        Grid.SetColumn(durationBorder, 3);
                        row.Children.Add(durationBorder);

                        rowBorder.Child = row;
                        rowsPanel.Children.Add(rowBorder);
                        idx++;
                    }

                    Grid.SetRow(rowsPanel, 1);
                    tableGrid.Children.Add(rowsPanel);

                    tableBorder.Child = tableGrid;
                    panel.Children.Add(tableBorder);

                    return panel;
                });

                // Group 1: General Physician
                var generalItems = prescription.Medicines.Where(item => item.Category == "General Physician" && !string.IsNullOrWhiteSpace(item.MedicineName)).ToList();
                if (generalItems.Count > 0)
                {
                    medicinesList.Children.Add(CreateTable("General Physician", generalItems));
                }

                // Group 2: Skin & Hair
                var skinHairItems = prescription.Medicines.Where(item => item.Category == "Skin & Hair" && !string.IsNullOrWhiteSpace(item.MedicineName)).ToList();
                if (skinHairItems.Count > 0)
                {
                    medicinesList.Children.Add(CreateTable("Skin & Hair", skinHairItems));
                }

                ScrollViewer scroll = new ScrollViewer { BorderThickness = new Thickness(0), Background = Brushes.Transparent, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled };
                scroll.Content = medicinesList;
                Grid.SetRow(scroll, 1);
                rxGrid.Children.Add(scroll);

                Grid.SetColumn(rxGrid, 2);
                bodyGrid.Children.Add(rxGrid);

                Grid.SetRow(bodyGrid, 2);
                mainGrid.Children.Add(bodyGrid);

                // --- ROW 3: Footer/Signature ---
                Grid footerGrid = new Grid { Margin = new Thickness(0, 30, 0, 0) };
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                Border sigBorder = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 1, 0, 0), Width = 180, Padding = new Thickness(0, 6, 0, 0) };
                sigBorder.Child = new TextBlock { Text = "Doctor's Signature", FontSize = 12, Foreground = Brushes.Black, HorizontalAlignment = HorizontalAlignment.Center, FontFamily = new FontFamily("Segoe UI") };
                Grid.SetColumn(sigBorder, 1);
                footerGrid.Children.Add(sigBorder);

                Grid.SetRow(footerGrid, 3);
                mainGrid.Children.Add(footerGrid);

                // Place grid in parent border
                pageBorder.Child = mainGrid;

                // Enforce layout measurement to standard A4 size
                pageBorder.Measure(new Size(A4Width, A4Height));
                pageBorder.Arrange(new Rect(new Point(0, 0), new Size(A4Width, A4Height)));
                pageBorder.UpdateLayout();

                // Send the layout to printer
                printDialog.PrintVisual(pageBorder, "Prescription Printout");
            }
        }
    }
}
