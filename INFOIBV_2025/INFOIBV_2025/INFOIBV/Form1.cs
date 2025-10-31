using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;

        /*
         * this enum defines the processing functions that will be shown in the dropdown (a.k.a. combobox)
         * you can expand it by adding new entries to applyProcessingFunction()
         */
        private enum ProcessingFunctions
        {
            Invert,
            AdjustContrast,
            ConvolveImage,
            DetectEdges,
            Threshold,
            BinaryErosion,
            BinaryDilation,
            BinaryOpening,
            BinaryClosing,
            GrayscaleErosion,
            GrayscaleDilation,
            HistogramEqualization,
            CheckHistogramProperties,
            SharpenImage,
            Task1,
            SequentialRegionLabeling,
            HoughTransform,
            HoughPeakFinding,
            HoughLineSegments,
            FloodFill,
            FindLineCrossings,
            BinaryPipeline,
            GrayscalePipeline,
            DetectTrafficSigns
        }

        /*
         * these are the parameters for your processing functions, you should add more as you see fit
         * it is useful to set them based on controls such as sliders, which you can add to the form
         */
        private byte filterSize = 5;
        private float filterSigma = 1f;
        private byte threshold = 127;

        private byte edgeMagnitudeThreshold = 100;
        private float peakThreshold = 0.5f;
        private int minSegmentLength = 30;
        private int maxGap = 3;


        public INFOIBV()
        {
            InitializeComponent();
            populateCombobox();

            // Set PictureBox modes to handle large images
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            
            // Initialize threshold numeric control
            numericThreshold.Minimum = 0;
            numericThreshold.Maximum = 255;
            numericThreshold.Value = threshold;
            numericThreshold.Visible = false;

            numericEdgeMagnitude.Value = edgeMagnitudeThreshold;
            numericPeakThreshold.Value = (decimal)peakThreshold;
            numericMinSegmentLength.Value = minSegmentLength;
            numericMaxGap.Value = maxGap;
        }

        /*
         * populateCombobox: populates the combobox with items as defined by the ProcessingFunctions enum
         */
        private void populateCombobox()
        {
            foreach (string itemName in Enum.GetNames(typeof(ProcessingFunctions)))
            {
                string ItemNameSpaces = Regex.Replace(Regex.Replace(itemName, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
                comboBox.Items.Add(ItemNameSpaces);
            }
            comboBox.SelectedIndex = 0;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all controls and labels first
            numericThreshold.Visible = false;
            labelThreshold.Visible = false;
            numericKernelSize.Visible = false;
            labelKernelSize.Visible = false;
            numericSigma.Visible = false;
            labelSigma.Visible = false;
            numericGrayscaleDilationSize.Visible = false;
            labelGrayscaleDilationSize.Visible = false;
            numericBinaryClosingSize.Visible = false;
            labelBinaryClosingSize.Visible = false;
            numericEdgeMagnitude.Visible = false;
            labelEdgeMagnitude.Visible = false;
            numericPeakThreshold.Visible = false;
            labelPeakThreshold.Visible = false;
            numericMinSegmentLength.Visible = false;
            labelMinSegmentLength.Visible = false;
            numericMaxGap.Visible = false;
            labelMaxGap.Visible = false;

            // Show controls based on selected function
            switch ((ProcessingFunctions)comboBox.SelectedIndex)
            {
                case ProcessingFunctions.Threshold:
                    numericThreshold.Visible = true;
                    labelThreshold.Visible = true;
                    break;

                case ProcessingFunctions.Task1:
                    numericKernelSize.Visible = true;
                    labelKernelSize.Visible = true;
                    numericSigma.Visible = true;
                    labelSigma.Visible = true;
                    break;

                case ProcessingFunctions.GrayscaleDilation:
                    numericGrayscaleDilationSize.Visible = true;
                    labelGrayscaleDilationSize.Visible = true;
                    break;

                case ProcessingFunctions.BinaryClosing:
                    numericBinaryClosingSize.Visible = true;
                    labelBinaryClosingSize.Visible = true;
                    break;

                case ProcessingFunctions.BinaryPipeline:
                    numericThreshold.Visible = true;
                    labelThreshold.Visible = true;
                    numericEdgeMagnitude.Visible = true;  // Add this for minimum intensity
                    labelEdgeMagnitude.Visible = true;     // Add this for minimum intensity
                    numericPeakThreshold.Visible = true;
                    labelPeakThreshold.Visible = true;
                    numericMinSegmentLength.Visible = true;
                    labelMinSegmentLength.Visible = true;
                    numericMaxGap.Visible = true;
                    labelMaxGap.Visible = true;
                    break;
                case ProcessingFunctions.GrayscalePipeline:
                    numericEdgeMagnitude.Visible = true;
                    labelEdgeMagnitude.Visible = true;          
                    numericPeakThreshold.Visible = true;
                    labelPeakThreshold.Visible = true;
                    numericMinSegmentLength.Visible = true;
                    labelMinSegmentLength.Visible = true;
                    numericMaxGap.Visible = true;
                    labelMaxGap.Visible = true;
                    break;
            }
        }

        /*
         * loadButton_Click: process when user clicks "Load" button
         */
        private void loadImageButton_Click(object sender, EventArgs e)
        {
            if (openImageDialog.ShowDialog() == DialogResult.OK)             // open file dialog
            {
                string file = openImageDialog.FileName;                     // get the file name
                imageFileName.Text = file;                                  // show file name
                if (InputImage != null) InputImage.Dispose();               // reset image
                InputImage = new Bitmap(file);                              // create new Bitmap from file
                //if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    //InputImage.Size.Height > 1500 || InputImage.Size.Width > 1500) // dimension check (may be removed or altered)
                    //MessageBox.Show("Error in image dimensions (have to be > 0 and <= 1024)");
                //else
                    pictureBox1.Image = (Image)InputImage;                 // display input image
            }
        }


        /*
         * applyButton_Click: process when user clicks "Apply" button
         */
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;
            if (OutputImage != null) OutputImage.Dispose();
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height];

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                    Image[x, y] = InputImage.GetPixel(x, y);                // set pixel color in array at (x,y)

            // execute image processing steps
            byte[,] workingImage = convertToGrayscale(Image);               // convert image to grayscale

            // Check if this needs RGB output
            if ((ProcessingFunctions)comboBox.SelectedIndex == ProcessingFunctions.HoughLineSegments)
            {
                byte[,,] rgbResult = applyHoughLineSegments(workingImage);
                OutputImage = new Bitmap(rgbResult.GetLength(1), rgbResult.GetLength(0));

                // copy RGB array to output Bitmap
                for (int x = 0; x < rgbResult.GetLength(1); x++)             // loop over columns
                    for (int y = 0; y < rgbResult.GetLength(0); y++)         // loop over rows
                    {
                        Color newColor = Color.FromArgb(rgbResult[y, x, 0], rgbResult[y, x, 1], rgbResult[y, x, 2]);
                        OutputImage.SetPixel(x, y, newColor);
                    }
            }
            else if ((ProcessingFunctions)comboBox.SelectedIndex == ProcessingFunctions.FindLineCrossings)
            {
                byte[,,] rgbResult = applyFindLineCrossings(workingImage);
                OutputImage = new Bitmap(rgbResult.GetLength(1), rgbResult.GetLength(0));

                // copy RGB array to output Bitmap
                for (int x = 0; x < rgbResult.GetLength(1); x++)             // loop over columns
                    for (int y = 0; y < rgbResult.GetLength(0); y++)         // loop over rows
                    {
                        Color newColor = Color.FromArgb(rgbResult[y, x, 0], rgbResult[y, x, 1], rgbResult[y, x, 2]);
                        OutputImage.SetPixel(x, y, newColor);
                    }
            }
            else if ((ProcessingFunctions)comboBox.SelectedIndex == ProcessingFunctions.BinaryPipeline)
            {
                byte[,,] rgbResult = applyBinaryPipeline(workingImage);
                OutputImage = new Bitmap(rgbResult.GetLength(1), rgbResult.GetLength(0));

                // copy RGB array to output Bitmap
                for (int x = 0; x < rgbResult.GetLength(1); x++)             // loop over columns
                    for (int y = 0; y < rgbResult.GetLength(0); y++)         // loop over rows
                    {
                        Color newColor = Color.FromArgb(rgbResult[y, x, 0], rgbResult[y, x, 1], rgbResult[y, x, 2]);
                        OutputImage.SetPixel(x, y, newColor);
                    }
            }
            else if ((ProcessingFunctions)comboBox.SelectedIndex == ProcessingFunctions.GrayscalePipeline)
            {
                byte[,,] rgbResult = applyGrayscalePipeline(workingImage);
                OutputImage = new Bitmap(rgbResult.GetLength(1), rgbResult.GetLength(0));
                
                // copy RGB array to output Bitmap
                for (int x = 0; x < rgbResult.GetLength(1); x++)             // loop over columns
                    for (int y = 0; y < rgbResult.GetLength(0); y++)         // loop over rows
                    {
                        Color newColor = Color.FromArgb(rgbResult[y, x, 0], rgbResult[y, x, 1], rgbResult[y, x, 2]);
                        OutputImage.SetPixel(x, y, newColor);
                    }
            }
            else if ((ProcessingFunctions)comboBox.SelectedIndex == ProcessingFunctions.DetectTrafficSigns)
            {
                byte[,,] rgbResult = detectTrafficSigns(workingImage);
                OutputImage = new Bitmap(rgbResult.GetLength(1), rgbResult.GetLength(0));
                
                for (int x = 0; x < rgbResult.GetLength(1); x++)
                    for (int y = 0; y < rgbResult.GetLength(0); y++)
                    {
                        Color newColor = Color.FromArgb(rgbResult[y, x, 0], rgbResult[y, x, 1], rgbResult[y, x, 2]);
                        OutputImage.SetPixel(x, y, newColor);
                    }
            }
            else
            {
                workingImage = applyProcessingFunction(workingImage);           // processing functions
                OutputImage = new Bitmap(workingImage.GetLength(0), workingImage.GetLength(1));

                // copy array to output Bitmap
                for (int x = 0; x < workingImage.GetLength(0); x++)             // loop over columns
                    for (int y = 0; y < workingImage.GetLength(1); y++)         // loop over rows
                    {
                        Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                        OutputImage.SetPixel(x, y, newColor);                  // set the pixel color at coordinate (x,y)
                    }
            }

            // Hide all controls and labels after processing
            numericThreshold.Visible = false;
            labelThreshold.Visible = false;
            numericKernelSize.Visible = false;
            labelKernelSize.Visible = false;
            numericSigma.Visible = false;
            labelSigma.Visible = false;
            numericBinaryClosingSize.Visible = false;
            labelBinaryClosingSize.Visible = false;
            numericGrayscaleDilationSize.Visible = false;
            labelGrayscaleDilationSize.Visible = false;
            numericEdgeMagnitude.Visible = false;
            labelEdgeMagnitude.Visible = false;
            numericPeakThreshold.Visible = false;
            labelPeakThreshold.Visible = false;
            numericMinSegmentLength.Visible = false;
            labelMinSegmentLength.Visible = false;
            numericMaxGap.Visible = false;
            labelMaxGap.Visible = false;
            pictureBox2.Image = (Image)OutputImage;                         // display output image
        }

        /*
         * applyProcessingFunction: defines behavior of function calls when "Apply" is pressed
         */
        private byte[,] applyProcessingFunction(byte[,] workingImage)
        {
            switch ((ProcessingFunctions)comboBox.SelectedIndex)
            {
                case ProcessingFunctions.Invert:
                    return invertImage(workingImage);
                case ProcessingFunctions.AdjustContrast:
                    return adjustContrast(workingImage);
                case ProcessingFunctions.ConvolveImage:
                    float[,] filter = createGaussianFilter(filterSize, filterSigma);
                    return convolveImage(workingImage, filter);
                case ProcessingFunctions.DetectEdges:
                    sbyte[,] horizontalKernel = null;
                    sbyte[,] verticalKernel = null;
                    return edgeMagnitude(workingImage, horizontalKernel, verticalKernel);
                case ProcessingFunctions.Threshold:
                    return thresholdImage(workingImage, threshold);

                case ProcessingFunctions.BinaryErosion:
                    bool[,] structElem = null;
                    return binaryErodeImage(workingImage, structElem);

                case ProcessingFunctions.BinaryDilation:
                    structElem = null;
                    return binaryDilateImage(workingImage, structElem);

                case ProcessingFunctions.BinaryOpening:
                    structElem = null;
                    return binaryOpenImage(workingImage, structElem);

                case ProcessingFunctions.BinaryClosing:
                    int closingSize = (int)numericBinaryClosingSize.Value;
                    bool[,] structElement = new bool[closingSize, closingSize];
                    for (int i = 0; i < closingSize; i++)
                        for (int j = 0; j < closingSize; j++)
                            structElement[i, j] = true;

                    byte[,] closedImage = binaryCloseImage(workingImage, structElement);

                    int foregroundCount = 0;
                    for (int x = 0; x < closedImage.GetLength(0); x++)
                        for (int y = 0; y < closedImage.GetLength(1); y++)
                            if (closedImage[x, y] == 255)
                                foregroundCount++;

                    MessageBox.Show($"Structuring element size: {closingSize}x{closingSize}\nForeground pixels: {foregroundCount}", "Binary Closing Stats");

                    return closedImage;

                case ProcessingFunctions.GrayscaleErosion:
                    int[,] grayStructElem = null;
                    return grayscaleErodeImage(workingImage, grayStructElem);

                case ProcessingFunctions.HistogramEqualization:
                    return histogramEqualization(workingImage);

                case ProcessingFunctions.CheckHistogramProperties:
                    ShowHistogramProperties(workingImage);
                    return workingImage;

                case ProcessingFunctions.SharpenImage:
                    return sharpenImage(workingImage);

                case ProcessingFunctions.Task1:
                    byte kernelSize = (byte)numericKernelSize.Value;
                    float sigma = (float)numericSigma.Value;
                    float[,] gaussian = createGaussianFilter(kernelSize, sigma);
                    byte[,] blurred = convolveImage(workingImage, gaussian);
                    byte[,] edges = edgeMagnitude(blurred, SobelX, SobelY);
                    byte thresholdValue = 100; // Fixed threshold
                    return thresholdImage(edges, thresholdValue);

                case ProcessingFunctions.GrayscaleDilation:
                    int size = (int)numericGrayscaleDilationSize.Value;
                    int[,] structElement2 = new int[size, size];
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            structElement2[i, j] = 1;
                    byte[,] result = grayscaleDilateImage(workingImage, structElement2);

                    // Show stats for the result
                    var (distinct, avg) = GetGrayscaleStats(result);
                    MessageBox.Show($"Distinct grayscale values: {distinct}\nAverage intensity: {avg:F2}", "Grayscale Dilation Stats");

                    return result;

                case ProcessingFunctions.SequentialRegionLabeling:
                    int regionCount;
                    byte[,] labeledImage = sequentialRegionLabeling(workingImage, out regionCount);

                    byte[,] enhancedLabeled = enhanceLabelVisualization(labeledImage, regionCount);


                    MessageBox.Show($"Total number of foreground regions: {regionCount}", "Sequential Region Labeling Results");

                    return enhancedLabeled;

                case ProcessingFunctions.HoughTransform:
                    float[,] accumulator = houghTransform(workingImage);

                    byte[,] houghImage = convertAccumulatorToImage(accumulator);

                    MessageBox.Show($"Hough Transform completed.\nAccumulator size: {accumulator.GetLength(0)} x {accumulator.GetLength(1)}\n(r-theta space)", "Hough Transform Results");

                    return houghImage;

                case ProcessingFunctions.HoughPeakFinding:
                    float[,] houghAccumulator = houghTransform(workingImage);

                    float peakThreshold = 0.6f; // 60% of maximum value
                    var peaks = peakFinding(houghAccumulator, peakThreshold);

                    byte[,] peaksImage = createPeaksVisualization(houghAccumulator, peaks);

                    string peakInfo = $"Peaks detected: {peaks.Count}\nThreshold used: {peakThreshold:P0} of max value\n\nPeak details:\n";

                    int maxPeaksToShow = Math.Min(5, peaks.Count);
                    for (int i = 0; i < maxPeaksToShow; i++)
                    {
                        var peak = peaks[i];
                        int rMax = (houghAccumulator.GetLength(0) - 1) / 2;
                        int actualR = peak.rIndex - rMax;
                        peakInfo += $"Peak {i + 1}: r={actualR}, θ={peak.thetaIndex}°, votes={peak.value:F1}\n";
                    }

                    if (peaks.Count > maxPeaksToShow)
                    {
                        peakInfo += $"... and {peaks.Count - maxPeaksToShow} more peaks";
                    }

                    MessageBox.Show(peakInfo, "Hough Peak Finding Results");

                    return peaksImage;

                case ProcessingFunctions.FloodFill:
                    int floodRegionCount;
                    byte[,] floodFilledImage = floodFill(workingImage, out floodRegionCount);

                    byte[,] enhancedFloodFill = enhanceLabelVisualization(floodFilledImage, floodRegionCount);

                    MessageBox.Show($"Total number of foreground regions: {floodRegionCount}", "Flood Fill Results");

                    return enhancedFloodFill;

                case ProcessingFunctions.BinaryPipeline:
                    return workingImage;

                case ProcessingFunctions.FindLineCrossings:
                    return workingImage;

                case ProcessingFunctions.HoughLineSegments:
                    return workingImage;

                case ProcessingFunctions.GrayscalePipeline:
                    return workingImage;

                default:
                    return null;
            }
        }


        /*
         * saveButton_Click: process when user clicks "Save" button
         */
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // save the output image
        }


        /*
         * convertToGrayScale: convert a three-channel color image to a single channel grayscale image
         * input:   inputImage          three-channel (Color) image
         * output:                      single-channel (byte) image
         */
        private byte[,] convertToGrayscale(Color[,] inputImage)
        {
            // create temporary grayscale image of the same size as input, with a single channel
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // process all pixels in the image
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                {
                    Color pixelColor = inputImage[x, y];                    // get pixel color
                    byte average = (byte)((pixelColor.R + pixelColor.B + pixelColor.G) / 3); // calculate average over the three channels
                    tempImage[x, y] = average;                              // set the new pixel color at coordinate (x,y)
                    progressBar.PerformStep();                              // increment progress bar
                }

            progressBar.Visible = false;                                    // hide progress bar

            return tempImage;
        }


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 1 GO HERE ==============
        // ====================================================================

        /*
         * invertImage: invert a single channel (grayscale) image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] invertImage(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            for (int x = 0; x < inputImage.GetLength(0); x++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                    tempImage[x, y] = (byte)(255 - inputImage[x, y]);     
            
            return tempImage;
        }


        /*
         * adjustContrast: create an image with the full range of intensity values used
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] adjustContrast(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            int low = 255;
            int high = 0;

            for (int x = 0; x < inputImage.GetLength(0); x++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                {
                    if (inputImage[x, y] > high) high = inputImage[x, y];
                    if (inputImage[x, y] < low) low = inputImage[x, y];
                }

            if (high == low)
            {
                for (int x = 0; x < inputImage.GetLength(0); x++)
                    for (int y = 0; y < inputImage.GetLength(1); y++)
                        tempImage[x, y] = inputImage[x, y];
                return tempImage;
            }

            for (int x = 0; x < inputImage.GetLength(0); x++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                {
                    tempImage[x, y] = (byte)((inputImage[x, y] - low) * 255 / (high - low));
                }

            return tempImage;
        }


        /*
         * createGaussianFilter: create a Gaussian filter of specific square size and with a specified sigma
         * input:   size                length and width of the Gaussian filter (only odd sizes)
         *          sigma               standard deviation of the Gaussian distribution
         * output:                      Gaussian filter
         */
        private float[,] createGaussianFilter(byte size, float sigma)
        {
            // create temporary grayscale image
            float[,] filter = new float[size, size];

            if (size % 2 == 0)
                throw new ArgumentException("Kernel size must be odd.");

            int half = size / 2;
            float sum = 0f;

            float sigma2 = 2 * sigma * sigma;

            for (int y = -half; y <= half; y++)
            {
                for (int x = -half; x <= half; x++)
                {
                    float value = (float)Math.Exp(-(x * x + y * y) / sigma2);
                    filter[y + half, x + half] = value;
                    sum += value;
                }
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    filter[y, x] /= sum;
                }
            }

            return filter;
        }


        /*
         * convolveImage: apply linear filtering of an input image
         * input:   inputImage          single-channel (byte) image
         *          filter              linear kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] convolveImage(byte[,] inputImage, float[,] filter)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            int filterSize = filter.GetLength(0);
            int filterOffset = filterSize / 2;
            for (int x = 0; x < inputImage.GetLength(0); x++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                {
                    float newValue = 0;
                    for (int fx = 0; fx < filterSize; fx++)
                        for (int fy = 0; fy < filterSize; fy++)
                        {
                            int imageX = x + fx - filterOffset;
                            int imageY = y + fy - filterOffset;
                            if (imageX >= 0 && imageX < inputImage.GetLength(0) && imageY >= 0 && imageY < inputImage.GetLength(1))
                            {
                                newValue += inputImage[imageX, imageY] * filter[fx, fy];
                            }
                        }
                    newValue = Math.Min(Math.Max(newValue, 0), 255);
                    tempImage[x, y] = (byte)newValue;
                }

            return tempImage;
        }


        /*
         * edgeMagnitude: calculate the image derivative of an input image and a provided edge kernel
         * input:   inputImage          single-channel (byte) image
         *          horizontalKernel    horizontal edge kernel
         *          verticalKernel      vertical edge kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] edgeMagnitude(byte[,] inputImage, sbyte[,] horizontalKernel, sbyte[,] verticalKernel)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            if (inputImage == null) throw new ArgumentNullException(nameof(inputImage));

            // Use default Sobel kernels if null
            if (horizontalKernel == null || verticalKernel == null)
            {
                horizontalKernel = new sbyte[,]
                {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
                };

                verticalKernel = new sbyte[,]
                {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
                };
            }

            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);
            int kSize = horizontalKernel.GetLength(0);
            int half = kSize / 2;

            byte[,] output = new byte[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int gx = 0, gy = 0;

                    for (int ky = 0; ky < kSize; ky++)
                    {
                        int iy = ReflectIndex(y + ky - half, h);
                        for (int kx = 0; kx < kSize; kx++)
                        {
                            int ix = ReflectIndex(x + kx - half, w);
                            byte pixel = inputImage[iy, ix];
                            gx += pixel * horizontalKernel[ky, kx];
                            gy += pixel * verticalKernel[ky, kx];
                        }
                    }

                    double mag = Math.Sqrt(gx * gx + gy * gy);
                    if (mag > 255) mag = 255;
                    output[y, x] = (byte)mag;
                }
            }

            return output;
        }

        private int ReflectIndex(int i, int n)
        {
            if (n <= 1) return 0;
            while (i < 0 || i >= n)
            {
                if (i < 0) i = -i - 1;
                else i = 2 * n - i - 1;
            }
            return i;
        }

        private readonly sbyte[,] SobelX = new sbyte[,]
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        // Vertical edges (gradient in Y direction)
        private readonly sbyte[,] SobelY = new sbyte[,]
        {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
        };


        /*
         * thresholdImage: threshold a grayscale image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image with on/off values
         */
        private byte[,] thresholdImage(byte[,] inputImage, byte threshold)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            for (int x = 0; x < inputImage.GetLength(0); x++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                {
                    if (inputImage[x, y] > threshold) tempImage[x, y] = 255;
                    else tempImage[x, y] = 0;
                }
            return tempImage;
        }

        // Binary morphology

        /*
         * binaryErodeImage: perform binary erosion on a binary image using a structuring element
         * input:   inputImage          single-channel (byte) binary image 
         *          structElem          binary structuring element (true = foreground)
         * output:                      single-channel (byte) binary image after erosion
         */
        private byte[,] binaryErodeImage(byte[,] inputImage, bool[,] structElem = null)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);

            // Default structuring element: 3x3 full square
            if (structElem == null)
            {
                int size = 11;
                structElem = new bool[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        structElem[i, j] = true;
                    }
                }
            }

            int k = structElem.GetLength(0);
            if (k != structElem.GetLength(1) || k % 2 == 0)
                throw new ArgumentException("Structuring element must be square with odd size.");

            int half = k / 2;
            byte[,] output = new byte[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool fits = true;

                    // Slide structuring element
                    for (int ky = 0; ky < k && fits; ky++)
                    {
                        for (int kx = 0; kx < k && fits; kx++)
                        {
                            if (structElem[ky, kx]) // only check active positions
                            {
                                int iy = y + ky - half;
                                int ix = x + kx - half;

                                // Outside treated as background
                                if (iy < 0 || iy >= h || ix < 0 || ix >= w ||
                                    inputImage[iy, ix] == 0)
                                {
                                    fits = false;
                                }
                            }
                        }
                    }

                    // Pixel remains foreground only if SE fits entirely
                    output[y, x] = fits ? (byte)255 : (byte)0;
                }
            }

            return output;
        }


        /*
         * binaryDilateImage: perform binary dilation on a binary image using a structuring element
         * input:   inputImage          single-channel (byte) binary image 
         *          structElem          binary structuring element (true = foreground)
         * output:                      single-channel (byte) binary image after dilation
         */
        private byte[,] binaryDilateImage(byte[,] inputImage, bool[,] structElem = null)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);

            if (structElem == null)
            {
                int size = 11;
                structElem = new bool[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        structElem[i, j] = true;
                    }
                }
            }

            int k = structElem.GetLength(0);
            if (k != structElem.GetLength(1) || k % 2 == 0)
                throw new ArgumentException("Structuring element must be square with odd size.");

            int half = k / 2;
            byte[,] output = new byte[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool hit = false;

                    // Slide structuring element
                    for (int ky = 0; ky < k && !hit; ky++)
                    {
                        for (int kx = 0; kx < k && !hit; kx++)
                        {
                            if (structElem[ky, kx]) // only check active positions
                            {
                                int iy = y + ky - half;
                                int ix = x + kx - half;

                                // Inside bounds AND pixel is foreground
                                if (iy >= 0 && iy < h && ix >= 0 && ix < w &&
                                    inputImage[iy, ix] != 0)
                                {
                                    hit = true;
                                }
                            }
                        }
                    }

                    // Pixel becomes white if any SE position hit foreground
                    output[y, x] = hit ? (byte)255 : (byte)0;
                }
            }

            return output;
        }

        private (int distinctValues, double average) GetGrayscaleStats(byte[,] image)
        {
            HashSet<byte> values = new HashSet<byte>();
            long sum = 0;
            int h = image.GetLength(0);
            int w = image.GetLength(1);

            for (int x = 0; x < h; x++)
                for (int y = 0; y < w; y++)
                {
                    byte val = image[x, y];
                    values.Add(val);
                    sum += val;
                }

            int distinctValues = values.Count;
            double average = sum / (double)(h * w);
            return (distinctValues, average);
        }


        /*
         * binaryOpenImage: perform binary opening on a binary image
         * input:   inputImage          single-channel (byte) binary image 
         *          structElem          binary structuring element (true = foreground)
         * output:                      single-channel (byte) binary image after opening
         */
        private byte[,] binaryOpenImage(byte[,] inputImage, bool[,] structElem)
        {

            byte[,] eroded = binaryErodeImage(inputImage, structElem);

            byte[,] opened = binaryDilateImage(eroded, structElem);

            return opened;
        }

        /*
         * binaryCloseImage: perform binary closing on a binary image
         * input:   inputImage          single-channel (byte) binary image
         *          structElem          binary structuring element (true = foreground)
         * output:                      single-channel (byte) binary image after closing
         */
        private byte[,] binaryCloseImage(byte[,] inputImage, bool[,] structElem)
        {
            byte[,] dilated = binaryDilateImage(inputImage, structElem);

            byte[,] closed = binaryErodeImage(dilated, structElem);

            return closed;
        }

        // Grayscale morphology

        /*
         * grayscaleErodeImage: perform grayscale erosion on a grayscale image using a structuring element
         * input:   inputImage          single-channel (byte) grayscale image
         *          structElem          integer structuring element 
         * output:                      single-channel (byte) grayscale image after erosion
         */
        private byte[,] grayscaleErodeImage(byte[,] inputImage, int[,] structElem)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);

             if (structElem == null)
            {
                int size = 11;
                structElem = new int[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        structElem[i, j] = 1;
                    }
                }
            }

            int k = structElem.GetLength(0);
            if (k != structElem.GetLength(1) || k % 2 == 0)
                throw new ArgumentException("Structuring element must be square with odd size.");

            int half = k / 2;
            byte[,] output = new byte[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int minVal = 255;

                    // Slide structuring element
                    for (int ky = 0; ky < k; ky++)
                    {
                        for (int kx = 0; kx < k; kx++)
                        {
                            if (structElem[ky, kx] != 0) // only check active positions
                            {
                                int iy = y + ky - half;
                                int ix = x + kx - half;

                                if (iy >= 0 && iy < h && ix >= 0 && ix < w)
                                {
                                    int val = inputImage[iy, ix] - structElem[ky, kx];
                                    if (val < minVal)
                                        minVal = val;
                                }
                            }
                        }
                    }

                    if (minVal < 0) minVal = 0;
                    if (minVal > 255) minVal = 255;
                    output[y, x] = (byte)minVal;
                }
            }

            return output;
        }

        /*
         * grayscaleDilateImage: perform grayscale dilation on a grayscale image using a structuring element
         * input:   inputImage          single-channel (byte) grayscale image
         *          structElem          integer structuring element
         * output:                      single-channel (byte) grayscale image after dilation
         */
        private byte[,] grayscaleDilateImage(byte[,] inputImage, int[,] structElem)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);

            if (structElem == null)
            {
                int size = 11;
                structElem = new int[size, size];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        structElem[i, j] = 1;
                    }
                }
            }

            int k = structElem.GetLength(0);
            if (k != structElem.GetLength(1) || k % 2 == 0)
                throw new ArgumentException("Structuring element must be square with odd size.");

            int half = k / 2;
            byte[,] output = new byte[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int maxVal = 0;

                    // Slide structuring element
                    for (int ky = 0; ky < k; ky++)
                    {
                        for (int kx = 0; kx < k; kx++)
                        {
                            if (structElem[ky, kx] != 0) // only check active positions
                            {
                                int iy = y + ky - half;
                                int ix = x + kx - half;

                                if (iy >= 0 && iy < h && ix >= 0 && ix < w)
                                {
                                    int val = inputImage[iy, ix] + structElem[ky, kx];
                                    if (val > maxVal)
                                        maxVal = val;
                                }
                            }
                        }
                    }

                    if (maxVal < 0) maxVal = 0;
                    if (maxVal > 255) maxVal = 255;
                    output[y, x] = (byte)maxVal;
                }
            }

            return output;
        }
        /*
         * histogramEqualization: perform histogram equalization on a grayscale image
         * input:   inputImage   single-channel (byte) grayscale image
         * output:               single-channel (byte) grayscale image after histogram equalization
         */
        private byte[,] histogramEqualization(byte[,] inputImage)
        {
            int height = inputImage.GetLength(0);
            int width = inputImage.GetLength(1);
            int totalPixels = height * width;

            byte[,] output = new byte[height, width];

            // Step 1: Compute histogram
            int[] histogram = new int[256];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    histogram[inputImage[i, j]]++;
                }
            }

            // Step 2: Compute cumulative distribution function (CDF)
            int[] cdf = new int[256];
            cdf[0] = histogram[0];
            for (int k = 1; k < 256; k++)
            {
                cdf[k] = cdf[k - 1] + histogram[k];
            }

            // Step 3: Normalize CDF (map to [0,255])
            byte[] equalizedLUT = new byte[256];
            int cdfMin = 0;
            for (int k = 0; k < 256; k++)
            {
                if (cdf[k] > 0)
                {
                    cdfMin = cdf[k];
                    break;
                }
            }
            for (int k = 0; k < 256; k++)
            {
                equalizedLUT[k] = (byte)Math.Round(((cdf[k] - cdfMin) / (double)(totalPixels - cdfMin)) * 255.0);
            }

            // Step 4: Map original intensities using equalizedLUT
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = equalizedLUT[inputImage[i, j]];
                }
            }

            return output;
        }

        private void ShowHistogramProperties(byte[,] image)
        {
            var (hasMaxContrast, hasMaxDynamicRange) = checkHistogramProperties(image);
            string message = $"Max Contrast: {(hasMaxContrast ? "Yes" : "No")}\nMax Dynamic Range: {(hasMaxDynamicRange ? "Yes" : "No")}";
            MessageBox.Show(message, "Histogram Properties");
        }

        /*
         * checkHistogramProperties: check if the image histogram has maximum contrast and maximum dynamic range
         * input:   inputImage   single-channel (byte) grayscale image
         * output:  (bool hasMaxContrast, bool hasMaxDynamicRange)
         */
        private (bool, bool) checkHistogramProperties(byte[,] inputImage)
        {
            int height = inputImage.GetLength(0);
            int width = inputImage.GetLength(1);

            // Step 1: Compute histogram
            int[] histogram = new int[256];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    histogram[inputImage[i, j]]++;
                }
            }

            // Step 2: Find min and max intensity present in the image
            int minIntensity = -1;
            int maxIntensity = -1;

            for (int k = 0; k < 256; k++)
            {
                if (histogram[k] > 0)
                {
                    minIntensity = k;
                    break;
                }
            }
            for (int k = 255; k >= 0; k--)
            {
                if (histogram[k] > 0)
                {
                    maxIntensity = k;
                    break;
                }
            }

            // Step 3: Check conditions
            bool hasMaxDynamicRange = (minIntensity == 0 && maxIntensity == 255);

            // For simplicity, define max contrast = histogram only at extremes (0 and 255)
            bool hasMaxContrast = (histogram[0] > 0 && histogram[255] > 0);
            for (int k = 1; k < 255; k++)
            {
                if (histogram[k] > 0)
                {
                    hasMaxContrast = false;
                    break;
                }
            }

            return (hasMaxContrast, hasMaxDynamicRange);
        }

        /*
         * sharpenImage: perform edge sharpening on a grayscale image
         * input:   inputImage   single-channel (byte) grayscale image
         * output:               single-channel (byte) sharpened image
         */
        private byte[,] sharpenImage(byte[,] inputImage)
        {
            // We will use a combination of Laplacian and identity kernels to get the sharpening kernel
            float[,] sharpenKernel = new float[,]
            {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            };

            byte[,] sharpenedImage = convolveImage(inputImage, sharpenKernel);

            return sharpenedImage;
        }

        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 2 GO HERE ==============
        // ====================================================================

        /*
         * sequentialRegionLabeling:
         * Perform connected-component labeling on a binary image (8-neighborhood)
         * Foreground pixels: 1 (or 255), background: 0
         * Output: grayscale label image, labels start from 2
         */
        private byte[,] sequentialRegionLabeling(byte[,] inputImage, out int regionCount)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);
            byte[,] labels = new byte[h, w];

            byte currentLabel = 2;

            // 8-neighborhood offsets
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Foreground pixel, not labeled yet?
                    if (inputImage[y, x] != 0 && labels[y, x] == 0)
                    {
                        Queue<(int, int)> q = new Queue<(int, int)>();
                        q.Enqueue((y, x));
                        labels[y, x] = currentLabel;

                        while (q.Count > 0)
                        {
                            var (cy, cx) = q.Dequeue();

                            // Check all 8 neighbors
                            for (int i = 0; i < 8; i++)
                            {
                                int ny = cy + dy[i];
                                int nx = cx + dx[i];

                                if (ny >= 0 && ny < h && nx >= 0 && nx < w)
                                {
                                    if (inputImage[ny, nx] != 0 && labels[ny, nx] == 0)
                                    {
                                        labels[ny, nx] = currentLabel;
                                        q.Enqueue((ny, nx));
                                    }
                                }
                            }
                        }

                        currentLabel++;

                        if (currentLabel == 255)
                            break;
                    }
                }
            }

            regionCount = currentLabel - 2;
            return labels;
        }

        /*
        * enhanceLabelVisualization: scale label values to use full grayscale range for better visualization
        * input:   labelImage   labeled image with labels starting from 2
        *          regionCount  total number of regions
        * output:              enhanced labeled image with better contrast
        */
        private byte[,] enhanceLabelVisualization(byte[,] labelImage, int regionCount)
        {
            int h = labelImage.GetLength(0);
            int w = labelImage.GetLength(1);
            byte[,] enhanced = new byte[h, w];
            
            if (regionCount == 0)
                return labelImage;
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (labelImage[y, x] == 0)
                    {
                        enhanced[y, x] = 0; // background stays black
                    }
                    else
                    {
                        byte scaledValue = (byte)(((labelImage[y, x] - 2) * 254) / Math.Max(1, regionCount - 1) + 1);
                        enhanced[y, x] = scaledValue;
                    }
                }
            }
            
            return enhanced;
        }

        /*
         * houghTransform:
         * Perform Hough transform for line detection.
         * Input:  edgeImage (binary or grayscale)
         * Output: 2D accumulator array (r-theta image)
         */
        private float[,] houghTransform(byte[,] edgeImage)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);

            // 1. Define Hough parameter space
            double diag = Math.Sqrt(h * h + w * w);
            int rMax = (int)Math.Ceiling(diag);
            int rRange = 2 * rMax + 1;
            int thetaRange = 180;

            float[,] accumulator = new float[rRange, thetaRange];

            // 2. Precompute sin/cos for efficiency
            double[] cosTheta = new double[thetaRange];
            double[] sinTheta = new double[thetaRange];
            for (int t = 0; t < thetaRange; t++)
            {
                double thetaRad = t * Math.PI / 180.0;
                cosTheta[t] = Math.Cos(thetaRad);
                sinTheta[t] = Math.Sin(thetaRad);
            }

            // 3. Define image center as origin
            int xCenter = w / 2;
            int yCenter = h / 2;

            // 4. Loop through all edge pixels
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    byte pixel = edgeImage[y, x];
                    if (pixel == 0) continue; // skip background

                    // For grayscale edge map, use intensity as vote weight
                    float vote = pixel / 255.0f;
                    if (vote == 0) vote = 1; 

                    // Adjust coordinates so origin is at image center
                    int xShift = x - xCenter;
                    int yShift = y - yCenter;

                    for (int t = 0; t < thetaRange; t++)
                    {
                        double r = xShift * cosTheta[t] + yShift * sinTheta[t];
                        int rIndex = (int)Math.Round(r) + rMax;

                        if (rIndex >= 0 && rIndex < rRange)
                        {
                            accumulator[rIndex, t] += vote;
                        }
                    }
                }
            }

            return accumulator;
        }

        /*
        * convertAccumulatorToImage: convert Hough accumulator array to displayable byte image
        * input:   accumulator     2D float array from Hough transform
        * output:                  byte image for visualization
        */
        private byte[,] convertAccumulatorToImage(float[,] accumulator)
        {
            int h = accumulator.GetLength(0);
            int w = accumulator.GetLength(1);
            byte[,] image = new byte[h, w];
            
            // Find min and max values in accumulator
            float minVal = float.MaxValue;
            float maxVal = float.MinValue;
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (accumulator[y, x] < minVal) minVal = accumulator[y, x];
                    if (accumulator[y, x] > maxVal) maxVal = accumulator[y, x];
                }
            }
            
            // Handle edge case where all values are the same
            if (maxVal == minVal)
            {
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        image[y, x] = 128; // mid-gray
                return image;
            }
            
            float range = maxVal - minVal;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float normalized = (accumulator[y, x] - minVal) / range;
                    image[y, x] = (byte)(normalized * 255);
                }
            }
            
            return image;
        }

        /*
         * peakFinding:
         * Identify peaks in the Hough accumulator using threshold and non-maximum suppression.
         * Input:  accumulator (r-theta image)
         *         t_peak      (threshold; can be raw or percentage [0..1])
         * Output: list of peaks (rIndex, thetaIndex, value)
         */
        private List<(int rIndex, int thetaIndex, float value)> peakFinding(float[,] accumulator, float t_peak)
        {
            int rCount = accumulator.GetLength(0);
            int tCount = accumulator.GetLength(1);
            List<(int, int, float)> peaks = new List<(int, int, float)>();

            // 1. Find global max for threshold normalization
            float maxVal = 0;
            for (int r = 0; r < rCount; r++)
                for (int t = 0; t < tCount; t++)
                    if (accumulator[r, t] > maxVal)
                        maxVal = accumulator[r, t];

            float threshold = (t_peak <= 1.0f) ? t_peak * maxVal : t_peak;

            // 2. Non-maximum suppression window size
            int winR = 5;
            int winT = 5;

            // 3. Search for local maxima
            for (int r = 1; r < rCount - 1; r++)
            {
                for (int t = 1; t < tCount - 1; t++)
                {
                    float val = accumulator[r, t];
                    if (val < threshold) continue;

                    bool isMax = true;

                    for (int dy = -winR / 2; dy <= winR / 2 && isMax; dy++)
                    {
                        for (int dx = -winT / 2; dx <= winT / 2 && isMax; dx++)
                        {
                            int ry = r + dy;
                            int tx = (t + dx + tCount) % tCount;
                            if (ry < 0 || ry >= rCount) continue;

                            if (accumulator[ry, tx] > val)
                                isMax = false;
                        }
                    }

                    if (isMax)
                        peaks.Add((r, t, val));
                }
            }

            return peaks;
        }

        /*
        * createPeaksVisualization: create an image showing only the detected peaks
        * input:   accumulator     2D float array from Hough transform
        *          peaks          list of detected peaks
        * output:                 byte image showing only peaks (white) on black background
        */
        private byte[,] createPeaksVisualization(float[,] accumulator, List<(int rIndex, int thetaIndex, float value)> peaks)
        {
            int h = accumulator.GetLength(0);
            int w = accumulator.GetLength(1);
            byte[,] peaksImage = new byte[h, w];
            
            // Mark each peak as white (255)
            foreach (var (rIndex, thetaIndex, value) in peaks)
            {
                if (rIndex >= 0 && rIndex < h && thetaIndex >= 0 && thetaIndex < w)
                {
                    peaksImage[rIndex, thetaIndex] = 255;
                }
            }
            
            return peaksImage;
        }

        /*
        * createPeaksOverlayVisualization: create an image showing peaks overlaid on the accumulator
        * input:   accumulator     2D float array from Hough transform
        *          peaks          list of detected peaks
        * output:                 byte image showing accumulator with bright peaks highlighted
        */
        private byte[,] createPeaksOverlayVisualization(float[,] accumulator, List<(int rIndex, int thetaIndex, float value)> peaks)
        {
            byte[,] overlayImage = convertAccumulatorToImage(accumulator);
            
            int h = accumulator.GetLength(0);
            int w = accumulator.GetLength(1);
            
            // Highlight each peak and its immediate neighborhood
            foreach (var (rIndex, thetaIndex, value) in peaks)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int y = rIndex + dy;
                        int x = thetaIndex + dx;
                        
                        if (y >= 0 && y < h && x >= 0 && x < w)
                        {
                            overlayImage[y, x] = 255; // Bright white for peaks
                        }
                    }
                }
            }
            
            return overlayImage;
        }

        /*
        * houghLineSegments:
        * Detect and draw line segments for given Hough peaks.
        * Input:  edgeImage          -< binary or grayscale edge map
        *         peaks              -> list of (r, theta) peaks
        *         minThreshold       -> minimum edge intensity (0-255)
        *         minSegmentLength   -> minimum pixels per segment
        *         maxGap             -> maximum allowed gap between edge pixels
        * Output: RGB image with line segments overlaid in red
        */
        private byte[,,] houghLineSegments(
            byte[,] edgeImage,
            List<(int rIndex, int thetaIndex, float value)> peaks,
            float minThreshold,
            int minSegmentLength,
            int maxGap)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);

            byte[,,] output = new byte[h, w, 3];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    output[y, x, 0] = output[y, x, 1] = output[y, x, 2] = edgeImage[y, x];

            double diag = Math.Sqrt(h * h + w * w);
            int rMax = (int)Math.Ceiling(diag);
            int thetaRange = 180;

            int xCenter = w / 2;
            int yCenter = h / 2;

            foreach (var (rIdx, tIdx, _) in peaks)
            {
                double theta = tIdx * Math.PI / 180.0;
                double cosT = Math.Cos(theta);
                double sinT = Math.Sin(theta);
                double r = rIdx - rMax;

                bool isVertical = Math.Abs(cosT) < Math.Abs(sinT);

                List<(int, int)> currentSegment = new List<(int, int)>();
                int gapCount = 0;

                if (!isVertical)
                {
                    for (int x = 0; x < w; x++)
                    {
                        double yF = (r - (x - xCenter) * cosT) / sinT + yCenter;
                        int y = (int)Math.Round(yF);

                        if (y >= 0 && y < h)
                        {
                            byte val = edgeImage[y, x];
                            bool isEdge = val >= minThreshold;

                            if (isEdge)
                            {
                                currentSegment.Add((x, y));
                                gapCount = 0;
                            }
                            else if (currentSegment.Count > 0)
                            {
                                gapCount++;
                                if (gapCount > maxGap)
                                {
                                    if (currentSegment.Count >= minSegmentLength)
                                        DrawSegment(output, currentSegment);
                                    currentSegment.Clear();
                                    gapCount = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < h; y++)
                    {
                        double xF = (r - (y - yCenter) * sinT) / cosT + xCenter;
                        int x = (int)Math.Round(xF);

                        if (x >= 0 && x < w)
                        {
                            byte val = edgeImage[y, x];
                            bool isEdge = val >= minThreshold;

                            if (isEdge)
                            {
                                currentSegment.Add((x, y));
                                gapCount = 0;
                            }
                            else if (currentSegment.Count > 0)
                            {
                                gapCount++;
                                if (gapCount > maxGap)
                                {
                                    if (currentSegment.Count >= minSegmentLength)
                                        DrawSegment(output, currentSegment);
                                    currentSegment.Clear();
                                    gapCount = 0;
                                }
                            }
                        }
                    }
                }

                // Draw final segment if it’s long enough
                if (currentSegment.Count >= minSegmentLength)
                    DrawSegment(output, currentSegment);
            }

            return output;
        }

        /*
        * Helper function to draw a red line segment on the RGB image.
        */
        private void DrawSegment(byte[,,] image, List<(int x, int y)> segment)
        {
            foreach (var (x, y) in segment)
            {
                if (x >= 0 && x < image.GetLength(1) && y >= 0 && y < image.GetLength(0))
                {
                    image[y, x, 0] = 255; // Red
                    image[y, x, 1] = 0;
                    image[y, x, 2] = 0;
                }
            }
        }

        /*
        * applyHoughLineSegments: apply Hough line segment detection and return RGB result
        * input:   workingImage    single-channel (byte) grayscale image
        * output:                  RGB image with line segments overlaid
        */
        private byte[,,] applyHoughLineSegments(byte[,] workingImage)
        {
            // Apply Hough transform to detect lines
            float[,] accumulator = houghTransform(workingImage);

            // Find peaks in the accumulator
            float peakThreshold = 0.6f; // 60% of maximum value
            var peaks = peakFinding(accumulator, peakThreshold);

            // Set parameters for line segment detection
            float minThreshold = 80;      // minimum edge intensity
            int minSegmentLength = 40;    // minimum pixels per segment
            int maxGap = 5;              // maximum gap between edge pixels

            // Apply line segment detection
            byte[,,] segmentImage = houghLineSegments(workingImage, peaks, minThreshold, minSegmentLength, maxGap);

            string segmentInfo = $"Line segments detected from {peaks.Count} peaks\n" +
                                $"Parameters used:\n" +
                                $"- Peak threshold: {peakThreshold:P0} of max value\n" +
                                $"- Min edge intensity: {minThreshold}\n" +
                                $"- Min segment length: {minSegmentLength} pixels\n" +
                                $"- Max gap: {maxGap} pixels";

            MessageBox.Show(segmentInfo, "Hough Line Segments Results");

            return segmentImage;
        }

        /*
        * applyBinaryPipeline: complete binary line detection pipeline using GUI parameter controls
        * input:   workingImage    single-channel (byte) grayscale image
        * output:                  RGB image with detected line segments overlaid
        */
        private byte[,,] applyBinaryPipeline(byte[,] workingImage)
        {
            // Step 1: Gaussian smoothing
            byte gaussianSize = 5;
            float gaussianSigma = 1.0f;
            float[,] gaussianFilter = createGaussianFilter(gaussianSize, gaussianSigma);
            byte[,] smoothedImage = convolveImage(workingImage, gaussianFilter);

            // Step 2: Edge detection using Sobel operators
            byte[,] edgeImage = edgeMagnitude(smoothedImage, SobelX, SobelY);

            // Step 3: Apply threshold to obtain binary edge map
            byte t_mag = (byte)numericThreshold.Value;
            byte[,] binaryEdgeMap = thresholdImage(edgeImage, t_mag);

            // Step 4: Hough transform on binary edge map
            float[,] accumulator = houghTransform(binaryEdgeMap);

            // Step 5: Peak finding to identify the most salient lines
            float t_peak = (float)numericPeakThreshold.Value;
            var peaks = peakFinding(accumulator, t_peak);

            // Step 6: Line segment detection and visualization
            float minThreshold = (float)numericEdgeMagnitude.Value;
            int minSegLength = (int)numericMinSegmentLength.Value;
            int maxGapSize = (int)numericMaxGap.Value;

            byte[,,] resultImage = houghLineSegments(binaryEdgeMap, peaks, minThreshold, minSegLength, maxGapSize);

            string pipelineInfo = $"BINARY LINE DETECTION PIPELINE - PARAMETER REPORT\n" +
                                $"=========================================\n\n" +
                                $"INPUT IMAGE SIZE: {workingImage.GetLength(0)} x {workingImage.GetLength(1)}\n\n" +
                                $"STEP 1 - GAUSSIAN SMOOTHING (Fixed):\n" +
                                $"  • Kernel size: {gaussianSize} x {gaussianSize}\n" +
                                $"  • Sigma: {gaussianSigma:F2}\n\n" +
                                $"STEP 2 - EDGE DETECTION (Fixed):\n" +
                                $"  • Method: Sobel operators\n" +
                                $"  • Horizontal kernel: 3x3 Sobel X\n" +
                                $"  • Vertical kernel: 3x3 Sobel Y\n\n" +
                                $"STEP 3 - BINARY THRESHOLDING (User Adjustable):\n" +
                                $"  • Edge magnitude threshold (t_mag): {t_mag}\n" +
                                $"  • Control: NumericUpDown (0-255)\n\n" +
                                $"STEP 4 - HOUGH TRANSFORM (Fixed):\n" +
                                $"  • Accumulator size: {accumulator.GetLength(0)} x {accumulator.GetLength(1)}\n" +
                                $"  • r-range: -{(accumulator.GetLength(0) - 1) / 2} to +{(accumulator.GetLength(0) - 1) / 2}\n" +
                                $"  • θ-range: 0° to 179°\n" +
                                $"  • Origin: Image center\n\n" +
                                $"STEP 5 - PEAK DETECTION (User Adjustable):\n" +
                                $"  • Peak threshold (t_peak): {t_peak:F2}\n" +
                                $"  • Control: NumericUpDown (0.0-1.0)\n" +
                                $"  • Lines detected: {peaks.Count}\n\n" +
                                $"STEP 6 - LINE SEGMENT DETECTION (User Adjustable):\n" +
                                $"  • Minimum edge intensity: {minThreshold:F1}\n" +
                                $"  • Minimum segment length: {minSegLength} pixels\n" +
                                $"  • Maximum gap: {maxGapSize} pixels\n" +
                                $"  • Controls: NumericUpDown for all parameters\n\n" +
                                $"VISUALIZATION:\n" +
                                $"  • Line segments shown in red on grayscale background\n\n" +
                                $"REPRODUCIBILITY PARAMETERS:\n" +
                                $"  t_mag={t_mag}, t_peak={t_peak:F2}, min_intensity={minThreshold:F1}, min_length={minSegLength}, max_gap={maxGapSize}";

            MessageBox.Show(pipelineInfo, "Binary Pipeline - Parameter Report");

            return resultImage;
        }

        /*
        * applyGrayscalePipeline: complete grayscale line detection pipeline
        * input:   workingImage    single-channel (byte) grayscale image
        * output:                  RGB image with detected line segments overlaid
        */
        private byte[,,] applyGrayscalePipeline(byte[,] workingImage)
        {
            // Step 1: Gaussian smoothing 
            byte gaussianSize = 5;
            float gaussianSigma = 1.0f;
            float[,] gaussianFilter = createGaussianFilter(gaussianSize, gaussianSigma);
            byte[,] smoothedImage = convolveImage(workingImage, gaussianFilter);

            // Step 2: Edge detection using Sobel operators 
            byte[,] edgeImage = edgeMagnitude(smoothedImage, SobelX, SobelY);

            // Step 3: Hough transform on grayscale edge map
            float[,] accumulator = houghTransform(edgeImage);

            // Step 4: Peak finding
            float t_peak = (float)numericPeakThreshold.Value;
            var peaks = peakFinding(accumulator, t_peak);

            // Step 5: Line segment detection
            float minThreshold = (float)numericEdgeMagnitude.Value;
            int minSegLength = (int)numericMinSegmentLength.Value;
            int maxGapSize = (int)numericMaxGap.Value;

            byte[,,] resultImage = houghLineSegments(edgeImage, peaks, minThreshold, minSegLength, maxGapSize);

            string pipelineInfo = $"GRAYSCALE LINE DETECTION PIPELINE\n" +
                                $"===============================\n\n" +
                                $"FIXED PARAMETERS:\n" +
                                $"  • Gaussian kernel: {gaussianSize}x{gaussianSize}\n" +
                                $"  • Gaussian sigma: {gaussianSigma:F1}\n" +
                                $"  • Edge detection: Sobel 3x3\n\n" +
                                $"USER-ADJUSTABLE PARAMETERS:\n" +
                                $"  • Minimum intensity: {minThreshold:F0}\n" +
                                $"  • Peak threshold: {t_peak:F2}\n" +
                                $"  • Min segment length: {minSegLength}\n" +
                                $"  • Max gap: {maxGapSize}\n" +
                                $"  • Lines detected: {peaks.Count}\n\n" +
                                $"REPRODUCTION PARAMETERS:\n" +
                                $"min_intensity={minThreshold:F0}, peak_threshold={t_peak:F2}, " +
                                $"min_length={minSegLength}, max_gap={maxGapSize}";

            MessageBox.Show(pipelineInfo, "Grayscale Pipeline");

            return resultImage;
        }







        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 3 GO HERE ==============
        // ====================================================================

        /*
        * TrafficSignCandidate: Represents a potential traffic sign
        */
        private class TrafficSignCandidate
        {
            public string Type { get; set; }  // "Circle" or "Triangle"
            public int CenterX { get; set; }
            public int CenterY { get; set; }
            public double Radius { get; set; }  // For circles
            public List<(int x, int y)> Corners { get; set; }  // For triangles
            public double Confidence { get; set; }
            public double Compactness { get; set; }
            public int Area { get; set; }
        }

                /*
        * detectTrafficSigns: REVISED - Focus on finding actual circular boundaries
        */
        private byte[,,] detectTrafficSigns(byte[,] inputImage)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("=== TRAFFIC SIGN DETECTION PIPELINE (REVISED) ===\n");
            
            int originalH = inputImage.GetLength(0);
            int originalW = inputImage.GetLength(1);
            
            log.AppendLine($"Original size: {originalW} x {originalH}");
            
            // PHASE 0: DOWNSCALING
            int maxDimension = 800;
            byte[,] workingImage = downscaleImageGrayscale(inputImage, maxDimension);
            
            int h = workingImage.GetLength(0);
            int w = workingImage.GetLength(1);
            double scaleRatio = (double)originalW / w;
            
            log.AppendLine($"Working size: {w} x {h} (scale: {scaleRatio:F2}x)\n");
            
            // PHASE 1: MULTI-SCALE EDGE DETECTION
            // Try to catch edges at different intensity levels
            log.AppendLine("PHASE 1: Multi-threshold Edge Detection");
            
            // Apply histogram equalization first
            byte[,] equalized = histogramEqualization(workingImage);
            
            // Light smoothing
            float[,] gaussianFilter = createGaussianFilter(3, 0.8f);
            byte[,] smoothed = convolveImage(equalized, gaussianFilter);
            
            // Compute edge magnitude
            byte[,] edgeMag = edgeMagnitude(smoothed, SobelX, SobelY);
            
            saveDebugImage(edgeMag, "edge_magnitude.png");
            
            // Create THREE binary edge maps with different thresholds
            byte[,] edges1 = thresholdImage(edgeMag, calculateAdaptiveThreshold(edgeMag, 0.10)); // Top 10%
            byte[,] edges2 = thresholdImage(edgeMag, calculateAdaptiveThreshold(edgeMag, 0.20)); // Top 20%
            byte[,] edges3 = thresholdImage(edgeMag, calculateAdaptiveThreshold(edgeMag, 0.40)); // Top 40%
            
            log.AppendLine($"  Created 3 edge maps with thresholds: 10%, 20%, 40%");
            
            // Combine edge maps (OR operation)
            byte[,] combinedEdges = new byte[h, w];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    combinedEdges[y, x] = (byte)(Math.Max(edges1[y, x], Math.Max(edges2[y, x], edges3[y, x])));
            
            saveDebugImage(combinedEdges, "combined_edges.png");
            
            // PHASE 2: MORPHOLOGICAL PROCESSING
            log.AppendLine("\nPHASE 2: Morphological Operations");
            
            // SKIP closing - it destroys the circular structure!
            // Just use the combined edges directly
            byte[,] closed = combinedEdges; // Use edges directly, no closing!
            
            log.AppendLine("  → Skipping morphological closing (preserves circular edges)");
            
            saveDebugImage(closed, "closed_edges.png");
            
            // PHASE 3: CIRCLE DETECTION WITH GRADIENT DIRECTION
            log.AppendLine("\nPHASE 3: Circle Detection with Gradient Voting");
            
            // HUGE minimum radius - in 800px image, minimum is 40px
            // In original 2000px image, that would be 100px radius minimum
            int minRadius = Math.Max(40, (int)(80 / scaleRatio)); // 80px in ORIGINAL scale
            int maxRadius = Math.Max(minRadius + 40, (int)(250 / scaleRatio)); // 250px max in original
            
            log.AppendLine($"  Searching radius range: {minRadius}-{maxRadius} px (scaled)");
            log.AppendLine($"  Original scale: {(int)(minRadius*scaleRatio)}-{(int)(maxRadius*scaleRatio)} px");
            log.AppendLine($"  This is VERY LARGE - only major circular signs will be detected\n");
            
            var circleCandidates = detectCirclesWithGradient(smoothed, edgeMag, closed, minRadius, maxRadius, log);
            
            log.AppendLine($"\n  → Found {circleCandidates.Count} circle candidates");
            
            // PHASE 4: STRICT FILTERING
            log.AppendLine("\nPHASE 4: Filtering (VERY RELAXED)");
            
            var filteredCircles = new List<TrafficSignCandidate>();
            
            foreach (var circle in circleCandidates)
            {
                // Must be in upper 95% of image
                if (circle.CenterY > h * 0.95)
                {
                    log.AppendLine($"  Rejected: ({circle.CenterX},{circle.CenterY}) too low");
                    continue;
                }
                
                // Must have reasonable radius
                if (circle.Radius < minRadius || circle.Radius > maxRadius)
                {
                    log.AppendLine($"  Rejected: ({circle.CenterX},{circle.CenterY}) radius {circle.Radius} out of range");
                    continue;
                }
                
                // SUPER RELAXED thresholds: 20% compactness, 8% confidence
                if (circle.Compactness < 0.20 || circle.Confidence < 0.08)
                {
                    log.AppendLine($"  Rejected: ({circle.CenterX},{circle.CenterY}) compact={circle.Compactness:F2} conf={circle.Confidence:F2}");
                    continue;
                }
                
                log.AppendLine($"  ✓ ACCEPTED: ({circle.CenterX},{circle.CenterY}), r={circle.Radius}, compact={circle.Compactness:F2}, conf={circle.Confidence:F2}");
                filteredCircles.Add(circle);
            }
            
            log.AppendLine($"  → {filteredCircles.Count} circles after filtering");
            
            // PHASE 5: VISUALIZATION
            log.AppendLine("\nPHASE 5: Visualization");
            byte[,,] output = drawTrafficSigns(workingImage, filteredCircles, log);
            
            log.AppendLine($"\n=== FINAL: {filteredCircles.Count} traffic signs detected ===");
            log.AppendLine("\nDEBUG FILES:");
            log.AppendLine("  - edge_magnitude.png (raw edges)");
            log.AppendLine("  - combined_edges.png (multi-threshold)");
            log.AppendLine("  - closed_edges.png (after morphology)");
            
            MessageBox.Show(log.ToString(), "Traffic Sign Detection", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            return output;
        }

        /*
        * detectCirclesWithGradient: MORE RELAXED voting
        */
        private List<TrafficSignCandidate> detectCirclesWithGradient(
            byte[,] originalImage,
            byte[,] edgeMagnitude,
            byte[,] binaryEdges,
            int minRadius,
            int maxRadius,
            StringBuilder log)
        {
            int h = originalImage.GetLength(0);
            int w = originalImage.GetLength(1);
            
            List<TrafficSignCandidate> circles = new List<TrafficSignCandidate>();
            
            minRadius = Math.Max(40, minRadius);
            if (maxRadius < minRadius + 30)
                maxRadius = minRadius + 60;
            
            log.AppendLine($"    RELAXED CIRCLE DETECTION");
            log.AppendLine($"    Radius range: {minRadius} - {maxRadius} px\n");
            
            int radiusStep = Math.Max(5, (maxRadius - minRadius) / 10);
            
            for (int radius = minRadius; radius <= maxRadius; radius += radiusStep)
            {
                log.AppendLine($"    Testing radius {radius}px...");
                
                int[,] accumulator = new int[h, w];
                
                // Vote for ALL possible centers
                int edgePixelCount = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (binaryEdges[y, x] == 0) continue;
                        
                        edgePixelCount++;
                        
                        // Vote every 10 degrees
                        for (int angle = 0; angle < 360; angle += 10)
                        {
                            double rad = angle * Math.PI / 180.0;
                            int cx = x + (int)(radius * Math.Cos(rad));
                            int cy = y + (int)(radius * Math.Sin(rad));
                            
                            if (cx >= 10 && cx < w - 10 && cy >= 10 && cy < h - 10)
                            {
                                accumulator[cy, cx]++;
                            }
                        }
                    }
                }
                
                log.AppendLine($"      Edge pixels: {edgePixelCount}");
                
                if (edgePixelCount < 50)
                {
                    log.AppendLine($"      Skipping - too few edges");
                    continue;
                }
                
                // Find max votes
                int maxVotes = 0;
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        if (accumulator[y, x] > maxVotes)
                            maxVotes = accumulator[y, x];
                
                log.AppendLine($"      Max votes: {maxVotes}");
                
                // MUCH MORE RELAXED threshold - only need 30% of maximum possible
                int angleStep = 10;
                int maxPossibleVotes = 360 / angleStep; // 36 votes
                int threshold = (int)(maxPossibleVotes * 0.30); // 30% = ~11 votes
                
                log.AppendLine($"      Threshold: {threshold} votes (30% of {maxPossibleVotes})");
                
                int candidatesFound = 0;
                
                // Find local maxima
                for (int y = 20; y < h - 20; y++)
                {
                    for (int x = 20; x < w - 20; x++)
                    {
                        if (accumulator[y, x] < threshold) continue;
                        
                        // Check if local maximum in 15px window
                        bool isMax = true;
                        for (int dy = -15; dy <= 15 && isMax; dy++)
                        {
                            for (int dx = -15; dx <= 15 && isMax; dx++)
                            {
                                int ny = y + dy;
                                int nx = x + dx;
                                if (ny >= 0 && ny < h && nx >= 0 && nx < w)
                                {
                                    if (accumulator[ny, nx] > accumulator[y, x])
                                        isMax = false;
                                }
                            }
                        }
                        
                        if (!isMax) continue;
                        
                        // Validate perimeter
                        double compactness = validateCirclePerimeter(binaryEdges, x, y, radius);
                        double confidence = accumulator[y, x] / (double)maxPossibleVotes;
                        
                        log.AppendLine($"      Candidate ({x},{y}): votes={accumulator[y,x]}, compact={compactness:F2}, conf={confidence:F2}");
                        
                        // VERY RELAXED thresholds: 25% compactness, 10% confidence
                        if (compactness > 0.25 && confidence > 0.10)
                        {
                            circles.Add(new TrafficSignCandidate
                            {
                                Type = "Circle",
                                CenterX = x,
                                CenterY = y,
                                Radius = radius,
                                Confidence = confidence,
                                Compactness = compactness,
                                Area = (int)(Math.PI * radius * radius)
                            });
                            
                            candidatesFound++;
                            log.AppendLine($"        ✓ ACCEPTED (compact={compactness:F2}, conf={confidence:F2})");
                            
                            if (circles.Count >= 50) goto ExitSearch;
                        }
                        else
                        {
                            log.AppendLine($"        ✗ Rejected (compact={compactness:F2}, conf={confidence:F2})");
                        }
                    }
                }
                
                log.AppendLine($"      → {candidatesFound} candidates at radius {radius}\n");
            }
            
            ExitSearch:
            
            log.AppendLine($"    Total candidates: {circles.Count}");
            
            if (circles.Count == 0)
            {
                log.AppendLine($"\n    ⚠ WARNING: NO CIRCLES DETECTED!");
                log.AppendLine($"    Edges look good in combined_edges.png");
                log.AppendLine($"    Try reducing minRadius or increasing angle sampling");
            }
            
            // Merge similar circles
            circles = mergeSimilarCirclesStrict(circles, log);
            
            // Keep top 20 by confidence
            circles = circles.OrderByDescending(c => c.Confidence)
                            .ThenByDescending(c => c.Compactness)
                            .Take(20)
                            .ToList();
            
            log.AppendLine($"    FINAL: {circles.Count} circles\n");
            
            return circles;
        }

        /*
        * validateCirclePerimeter: Check what percentage of the circle perimeter has edges
        */
        private double validateCirclePerimeter(byte[,] edgeImage, int cx, int cy, int radius)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            int edgeCount = 0;
            int totalSamples = 0;
            
            // Sample every 3 degrees around perimeter
            for (int angle = 0; angle < 360; angle += 3)
            {
                double rad = angle * Math.PI / 180.0;
                
                // Check in a small window (±3 pixels) around expected position
                bool foundEdge = false;
                for (int dr = -3; dr <= 3 && !foundEdge; dr++)
                {
                    int r = radius + dr;
                    int x = cx + (int)(r * Math.Cos(rad));
                    int y = cy + (int)(r * Math.Sin(rad));
                    
                    if (x >= 0 && x < w && y >= 0 && y < h && edgeImage[y, x] > 0)
                        foundEdge = true;
                }
                
                totalSamples++;
                if (foundEdge) edgeCount++;
            }
            
            return totalSamples > 0 ? edgeCount / (double)totalSamples : 0;
        }

        /*
        * validateCircleCompactnessRelaxed: Relaxed validation
        */
        private double validateCircleCompactnessRelaxed(byte[,] edgeImage, int cx, int cy, int radius)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            int edgeCount = 0;
            int totalSamples = 0;
            
            // Sample every 2 degrees (180 samples instead of 360)
            for (int angle = 0; angle < 360; angle += 2)
            {
                double rad = angle * Math.PI / 180.0;
                
                bool foundEdge = false;
                for (int dr = -3; dr <= 3 && !foundEdge; dr++) // Wider search window
                {
                    int r = radius + dr;
                    int x = cx + (int)(r * Math.Cos(rad));
                    int y = cy + (int)(r * Math.Sin(rad));
                    
                    if (x >= 0 && x < w && y >= 0 && y < h)
                    {
                        if (edgeImage[y, x] > 0)
                            foundEdge = true;
                    }
                }
                
                totalSamples++;
                if (foundEdge)
                    edgeCount++;
            }
            
            return totalSamples > 0 ? edgeCount / (double)totalSamples : 0;
        }

        /*
        * filterTrafficSignsRelaxed: RELAXED filtering
        */
        private List<TrafficSignCandidate> filterTrafficSignsRelaxed(
            List<TrafficSignCandidate> candidates,
            int imageWidth,
            int imageHeight,
            double scaleRatio,
            int minArea,
            int maxArea,
            StringBuilder log)
        {
            List<TrafficSignCandidate> valid = new List<TrafficSignCandidate>();
            
            int rejected = 0;
            Dictionary<string, int> rejectionReasons = new Dictionary<string, int>
            {
                {"position", 0},
                {"size", 0},
                {"compactness", 0},
                {"confidence", 0}
            };
            
            int actualMinArea = Math.Max(minArea, 500); // Reduced from 800
            
            foreach (var sign in candidates)
            {
                // Filter 1: Position (upper 80% instead of 70%)
                if (sign.CenterY > imageHeight * 0.80)
                {
                    rejected++;
                    rejectionReasons["position"]++;
                    continue;
                }
                
                // Filter 2: Size
                if (sign.Area < actualMinArea || sign.Area > maxArea)
                {
                    rejected++;
                    rejectionReasons["size"]++;
                    continue;
                }
                
                // Filter 3: Compactness - RELAXED to 0.55
                if (sign.Compactness < 0.55)
                {
                    rejected++;
                    rejectionReasons["compactness"]++;
                    continue;
                }
                
                // Filter 4: Confidence - RELAXED to 0.45
                if (sign.Confidence < 0.45)
                {
                    rejected++;
                    rejectionReasons["confidence"]++;
                    continue;
                }
                
                valid.Add(sign);
            }
            
            log.AppendLine($"  Total candidates: {candidates.Count}");
            log.AppendLine($"  Rejected: {rejected}");
            log.AppendLine($"    - By position: {rejectionReasons["position"]}");
            log.AppendLine($"    - By size: {rejectionReasons["size"]}");
            log.AppendLine($"    - By compactness: {rejectionReasons["compactness"]}");
            log.AppendLine($"    - By confidence: {rejectionReasons["confidence"]}");
            log.AppendLine($"  Filters applied (RELAXED):");
            log.AppendLine($"    - Position: y < 80% height");
            log.AppendLine($"    - Size: {actualMinArea} < area < {maxArea} px²");
            log.AppendLine($"    - Compactness: > 0.55");
            log.AppendLine($"    - Confidence: > 0.45");
            
            return valid;
        }

        /*
        * saveDebugImage: Save intermediate images for debugging
        */
        private void saveDebugImage(byte[,] image, string filename)
        {
            try
            {
                int h = image.GetLength(0);
                int w = image.GetLength(1);
                
                Bitmap bmp = new Bitmap(w, h);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        byte val = image[y, x];
                        bmp.SetPixel(x, y, Color.FromArgb(val, val, val));
                    }
                }
                
                string path = Path.Combine(Path.GetTempPath(), filename);
                bmp.Save(path);
                bmp.Dispose();
            }
            catch
            {
                // Ignore save errors
            }
        }

        /*
        * validateCircleCompactnessStrict: MUCH stricter circle validation
        * Samples MORE points and requires MUCH higher edge coverage
        */
        private double validateCircleCompactnessStrict(byte[,] edgeImage, int cx, int cy, int radius)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            int edgeCount = 0;
            int totalSamples = 0;
            
            // Sample EVERY DEGREE around the circle perimeter (360 samples)
            for (int angle = 0; angle < 360; angle += 1) // Every degree!
            {
                double rad = angle * Math.PI / 180.0;
                
                // Check in a SMALL window around expected circle position (±2 pixels)
                bool foundEdge = false;
                for (int dr = -2; dr <= 2 && !foundEdge; dr++)
                {
                    int r = radius + dr;
                    int x = cx + (int)(r * Math.Cos(rad));
                    int y = cy + (int)(r * Math.Sin(rad));
                    
                    if (x >= 0 && x < w && y >= 0 && y < h)
                    {
                        if (edgeImage[y, x] > 0)
                            foundEdge = true;
                    }
                }
                
                totalSamples++;
                if (foundEdge)
                    edgeCount++;
            }
            
            return totalSamples > 0 ? edgeCount / (double)totalSamples : 0;
        }

        /*
        * countEdgePixelsInCircle: Count how many edge pixels are within the circular region
        */
        private int countEdgePixelsInCircle(byte[,] edgeImage, int cx, int cy, int radius)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            int count = 0;
            int radiusSquared = radius * radius;
            
            // Check all pixels in bounding box
            int minX = Math.Max(0, cx - radius - 3);
            int maxX = Math.Min(w - 1, cx + radius + 3);
            int minY = Math.Max(0, cy - radius - 3);
            int maxY = Math.Min(h - 1, cy + radius + 3);
            
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    int distSquared = dx * dx + dy * dy;
                    
                    // Check if pixel is near the circle perimeter (±3 pixels)
                    int innerRadiusSquared = (radius - 3) * (radius - 3);
                    int outerRadiusSquared = (radius + 3) * (radius + 3);
                    
                    if (distSquared >= innerRadiusSquared && distSquared <= outerRadiusSquared)
                    {
                        if (edgeImage[y, x] > 0)
                            count++;
                    }
                }
            }
            
            return count;
        }

        /*
        * mergeSimilarCirclesStrict: Merge circles with STRICTER distance threshold
        */
        private List<TrafficSignCandidate> mergeSimilarCirclesStrict(
            List<TrafficSignCandidate> circles,
            StringBuilder log)
        {
            List<TrafficSignCandidate> merged = new List<TrafficSignCandidate>();
            bool[] used = new bool[circles.Count];
            
            for (int i = 0; i < circles.Count; i++)
            {
                if (used[i]) continue;
                
                var best = circles[i];
                used[i] = true;
                
                // Find similar circles and keep the best one
                for (int j = i + 1; j < circles.Count; j++)
                {
                    if (used[j]) continue;
                    
                    double dist = Math.Sqrt(
                        Math.Pow(circles[i].CenterX - circles[j].CenterX, 2) +
                        Math.Pow(circles[i].CenterY - circles[j].CenterY, 2));
                    
                    double avgRadius = (circles[i].Radius + circles[j].Radius) / 2.0;
                    double radiusDiff = Math.Abs(circles[i].Radius - circles[j].Radius);
                    
                    // Same circle if centers very close (within radius) and radii similar
                    if (dist < avgRadius * 0.5 && radiusDiff < avgRadius * 0.3)
                    {
                        used[j] = true;
                        // Keep the one with better compactness
                        if (circles[j].Compactness > best.Compactness)
                            best = circles[j];
                    }
                }
                
                merged.Add(best);
            }
            
            if (merged.Count < circles.Count)
                log.AppendLine($"    Merged {circles.Count} → {merged.Count} circles");
            
            return merged;
        }

        /*
        * filterTrafficSignsScaled: Apply validation filters with scaled parameters
        */
        private List<TrafficSignCandidate> filterTrafficSignsScaled(
            List<TrafficSignCandidate> candidates,
            int imageWidth,
            int imageHeight,
            double scaleRatio,
            int minArea,
            int maxArea,
            StringBuilder log)
        {
            List<TrafficSignCandidate> valid = new List<TrafficSignCandidate>();
            
            int rejected = 0;
            Dictionary<string, int> rejectionReasons = new Dictionary<string, int>
            {
                {"position", 0},
                {"size", 0},
                {"compactness", 0},
                {"confidence", 0}
            };
            
            // Calculate actual minimum area (defined at the start so it's in scope for logging)
            int actualMinArea = Math.Max(minArea, 800); // At least 800 px² (was 500)
            
            foreach (var sign in candidates)
            {
                // Filter 1: Position (upper 70% of image)
                if (sign.CenterY > imageHeight * 0.7)
                {
                    rejected++;
                    rejectionReasons["position"]++;
                    continue;
                }
                
                // Filter 2: Size (scaled) - MUCH stricter minimum
                if (sign.Area < actualMinArea || sign.Area > maxArea)
                {
                    rejected++;
                    rejectionReasons["size"]++;
                    continue;
                }
                
                // Filter 3: Compactness - EVEN STRICTER
                if (sign.Compactness < 0.75) // Was 0.7, now 0.75
                {
                    rejected++;
                    rejectionReasons["compactness"]++;
                    continue;
                }
                
                // Filter 4: Confidence - EVEN STRICTER
                if (sign.Confidence < 0.60) // Was 0.5, now 0.60
                {
                    rejected++;
                    rejectionReasons["confidence"]++;
                    continue;
                }
                
                valid.Add(sign);
            }
            
            log.AppendLine($"  Total candidates: {candidates.Count}");
            log.AppendLine($"  Rejected: {rejected}");
            log.AppendLine($"    - By position: {rejectionReasons["position"]}");
            log.AppendLine($"    - By size: {rejectionReasons["size"]}");
            log.AppendLine($"    - By compactness: {rejectionReasons["compactness"]}");
            log.AppendLine($"    - By confidence: {rejectionReasons["confidence"]}");
            log.AppendLine($"  Filters applied:");
            log.AppendLine($"    - Position: y < 70% height");
            log.AppendLine($"    - Size: {actualMinArea} < area < {maxArea} px² (scaled)");
            log.AppendLine($"    - Compactness: > 0.75 (VERY STRICT)");
            log.AppendLine($"    - Confidence: > 0.60 (VERY STRICT)");
            
            return valid;
        }

        /*
        * calculateAdaptiveThreshold: Calculate threshold based on percentile
        * Input:   image - grayscale image
        *          topPercentile - keep top X% (e.g., 0.20 for top 20%)
        * Output:  threshold value
        */
        private byte calculateAdaptiveThreshold(byte[,] image, double topPercentile)
        {
            int h = image.GetLength(0);
            int w = image.GetLength(1);
            
            int[] histogram = new int[256];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    histogram[image[y, x]]++;
            
            int totalPixels = h * w;
            int targetPixels = (int)(totalPixels * topPercentile);
            
            int cumSum = 0;
            for (int i = 255; i >= 0; i--)
            {
                cumSum += histogram[i];
                if (cumSum >= targetPixels)
                    return (byte)i;
            }
            
            return 60; // Fallback
        }


        /*
        * validateCircleCompactness: Check how circular a detected circle is
        */
        private double validateCircleCompactness(byte[,] edgeImage, int cx, int cy, int radius)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            int edgeCount = 0;
            int totalSamples = 0;
            
            // Sample around the circle perimeter
            for (int angle = 0; angle < 360; angle += 2)
            {
                double rad = angle * Math.PI / 180.0;
                
                // Check in a small window around expected circle position
                for (int dr = -3; dr <= 3; dr++)
                {
                    int r = radius + dr;
                    int x = cx + (int)(r * Math.Cos(rad));
                    int y = cy + (int)(r * Math.Sin(rad));
                    
                    if (x >= 0 && x < w && y >= 0 && y < h)
                    {
                        totalSamples++;
                        if (edgeImage[y, x] > 0)
                            edgeCount++;
                    }
                }
            }
            
            return totalSamples > 0 ? edgeCount / (double)totalSamples : 0;
        }

        /*
        * mergeSimilarCircles: Remove duplicate detections
        */
        private List<TrafficSignCandidate> mergeSimilarCircles(
            List<TrafficSignCandidate> circles,
            StringBuilder log)
        {
            List<TrafficSignCandidate> merged = new List<TrafficSignCandidate>();
            bool[] used = new bool[circles.Count];
            
            for (int i = 0; i < circles.Count; i++)
            {
                if (used[i]) continue;
                
                var best = circles[i];
                used[i] = true;
                
                // Find similar circles and keep the best one
                for (int j = i + 1; j < circles.Count; j++)
                {
                    if (used[j]) continue;
                    
                    double dist = Math.Sqrt(
                        Math.Pow(circles[i].CenterX - circles[j].CenterX, 2) +
                        Math.Pow(circles[i].CenterY - circles[j].CenterY, 2));
                    
                    double radiusDiff = Math.Abs(circles[i].Radius - circles[j].Radius);
                    
                    // Same circle if centers close and radii similar
                    if (dist < 20 && radiusDiff < 10)
                    {
                        used[j] = true;
                        if (circles[j].Confidence > best.Confidence)
                            best = circles[j];
                    }
                }
                
                merged.Add(best);
            }
            
            log.AppendLine($"    Merged {circles.Count} → {merged.Count} circles");
            return merged;
        }

        /*
        * detectTriangularSigns: Detect triangular traffic signs
        */
        private List<TrafficSignCandidate> detectTriangularSigns(
            byte[,] edgeImage,
            StringBuilder log)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);
            
            List<TrafficSignCandidate> triangles = new List<TrafficSignCandidate>();
            
            // Region labeling
            int regionCount;
            byte[,] labels = sequentialRegionLabeling(edgeImage, out regionCount);
            
            log.AppendLine($"    Found {regionCount} regions");
            
            // Analyze each region
            for (byte label = 2; label < regionCount + 2 && label < 255; label++)
            {
                // Extract region pixels
                List<(int x, int y)> pixels = new List<(int x, int y)>();
                
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        if (labels[y, x] == label)
                            pixels.Add((x, y));
                
                if (pixels.Count < 100 || pixels.Count > 10000) continue;
                
                // Calculate bounding box
                int minX = pixels.Min(p => p.x);
                int maxX = pixels.Max(p => p.x);
                int minY = pixels.Min(p => p.y);
                int maxY = pixels.Max(p => p.y);
                
                int width = maxX - minX + 1;
                int height = maxY - minY + 1;
                double aspectRatio = (double)width / height;
                
                // Triangles should have aspect ratio near 1.0
                if (aspectRatio < 0.7 || aspectRatio > 1.4) continue;
                
                // Detect corners using Douglas-Peucker or simple angle detection
                var corners = detectCorners(pixels);
                
                if (corners.Count >= 3 && corners.Count <= 4)
                {
                    // Calculate compactness
                    int area = pixels.Count;
                    int perimeter = calculatePerimeter(pixels, labels, label, h, w);
                    double compactness = (4 * Math.PI * area) / Math.Max(perimeter * perimeter, 1);
                    
                    if (compactness > 0.3)
                    {
                        triangles.Add(new TrafficSignCandidate
                                        {
                            Type = "Triangle",
                            CenterX = (int)pixels.Average(p => p.x),
                            CenterY = (int)pixels.Average(p => p.y),
                            Corners = corners.Take(3).ToList(),
                            Confidence = compactness,
                            Compactness = compactness,
                            Area = area
                        });
                    }
                }
            }
            
            return triangles;
        }

        /*
        * detectCorners: Simple corner detection using angle changes
        */
        private List<(int x, int y)> detectCorners(List<(int x, int y)> pixels)
        {
            if (pixels.Count < 10) return new List<(int, int)>();
            
            // Find convex hull or boundary
            var boundary = extractBoundary(pixels);
            
            if (boundary.Count < 10) return new List<(int, int)>();
            
            List<(int x, int y)> corners = new List<(int x, int y)>();
            
            int windowSize = Math.Max(3, boundary.Count / 20);
            
            for (int i = 0; i < boundary.Count; i++)
            {
                int prevIdx = (i - windowSize + boundary.Count) % boundary.Count;
                int nextIdx = (i + windowSize) % boundary.Count;
                
                var prev = boundary[prevIdx];
                var curr = boundary[i];
                var next = boundary[nextIdx];
                
                // Calculate angle
                double angle = calculateAngle(prev, curr, next);
                
                // Sharp angle = corner
                if (angle < 140) // Less than 140 degrees
                {
                    corners.Add(curr);
                }
            }
            
            // Cluster nearby corners
            corners = clusterCorners(corners, 10);
            
            return corners;
        }

        /*
        * extractBoundary: Extract boundary pixels from region
        */
        private List<(int x, int y)> extractBoundary(List<(int x, int y)> pixels)
        {
            HashSet<(int, int)> pixelSet = new HashSet<(int, int)>(pixels);
            List<(int x, int y)> boundary = new List<(int x, int y)>();
            
            foreach (var (x, y) in pixels)
            {
                // Check if any neighbor is outside region
                bool isBoundary = false;
                for (int dy = -1; dy <= 1 && !isBoundary; dy++)
                {
                    for (int dx = -1; dx <= 1 && !isBoundary; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (!pixelSet.Contains((x + dx, y + dy)))
                            isBoundary = true;
                    }
                }
                
                if (isBoundary)
                    boundary.Add((x, y));
            }
            
            return boundary;
        }

        /*
        * calculateAngle: Calculate angle between three points
        */
        private double calculateAngle((int x, int y) p1, (int x, int y) p2, (int x, int y) p3)
        {
            double dx1 = p1.x - p2.x;
            double dy1 = p1.y - p2.y;
            double dx2 = p3.x - p2.x;
            double dy2 = p3.y - p2.y;
            
            double dot = dx1 * dx2 + dy1 * dy2;
            double len1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            double len2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);
            
            if (len1 == 0 || len2 == 0) return 180;
            
            double cosAngle = dot / (len1 * len2);
            cosAngle = Math.Max(-1, Math.Min(1, cosAngle));
            
            return Math.Acos(cosAngle) * 180 / Math.PI;
        }

        /*
        * clusterCorners: Merge nearby corner points
        */
        private List<(int x, int y)> clusterCorners(List<(int x, int y)> corners, int threshold)
        {
            if (corners.Count == 0) return corners;
            
            List<(int x, int y)> clustered = new List<(int x, int y)>();
            bool[] used = new bool[corners.Count];
            
            for (int i = 0; i < corners.Count; i++)
            {
                if (used[i]) continue;
                
                List<(int x, int y)> cluster = new List<(int x, int y)> { corners[i] };
                used[i] = true;
                
                for (int j = i + 1; j < corners.Count; j++)
                {
                    if (used[j]) continue;
                    
                    double dist = Math.Sqrt(
                        Math.Pow(corners[i].x - corners[j].x, 2) +
                        Math.Pow(corners[i].y - corners[j].y, 2));
                    
                    if (dist < threshold)
                    {
                        cluster.Add(corners[j]);
                        used[j] = true;
                    }
                }
                
                // Average position
                int avgX = (int)cluster.Average(p => p.x);
                int avgY = (int)cluster.Average(p => p.y);
                clustered.Add((avgX, avgY));
            }
            
            return clustered;
        }

        /*
        * calculatePerimeter: Calculate region perimeter
        */
        private int calculatePerimeter(List<(int x, int y)> pixels, byte[,] labels, byte label, int h, int w)
        {
            HashSet<(int, int)> pixelSet = new HashSet<(int, int)>(pixels);
            int perimeter = 0;
            
            foreach (var (x, y) in pixels)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h || !pixelSet.Contains((nx, ny)))
                        {
                            perimeter++;
                            break;
                        }
                    }
                }
            }
            
            return perimeter;
        }

        /*
        * filterTrafficSigns: Apply validation filters
        */
        private List<TrafficSignCandidate> filterTrafficSigns(
            List<TrafficSignCandidate> candidates,
            int imageWidth,
            int imageHeight,
            StringBuilder log)
        {
            List<TrafficSignCandidate> valid = new List<TrafficSignCandidate>();
            
            int rejected = 0;
            
            foreach (var sign in candidates)
            {
                // Filter 1: Position (upper 70% of image)
                if (sign.CenterY > imageHeight * 0.7)
                {
                    rejected++;
                    continue;
                }
                
                // Filter 2: Size
                if (sign.Area < 200 || sign.Area > 30000)
                {
                    rejected++;
                    continue;
                }
                
                // Filter 3: Compactness
                if (sign.Compactness < 0.5)
                {
                    rejected++;
                    continue;
                }
                
                // Filter 4: Confidence
                if (sign.Confidence < 0.3)
                {
                    rejected++;
                    continue;
                }
                
                valid.Add(sign);
            }
            
            log.AppendLine($"  Rejected: {rejected}");
            log.AppendLine($"  Filters applied:");
            log.AppendLine($"    - Position: y < 70% height");
            log.AppendLine($"    - Size: 200 < area < 30000 px");
            log.AppendLine($"    - Compactness: > 0.5");
            log.AppendLine($"    - Confidence: > 0.3");
            
            return valid;
        }

        /*
        * drawTrafficSigns: Visualize detected signs on RGB image
        */
        private byte[,,] drawTrafficSigns(
            byte[,] baseImage,
            List<TrafficSignCandidate> signs,
            StringBuilder log)
        {
            int h = baseImage.GetLength(0);
            int w = baseImage.GetLength(1);
            
            byte[,,] output = new byte[h, w, 3];
            
            // Copy grayscale to RGB
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    output[y, x, 0] = output[y, x, 1] = output[y, x, 2] = baseImage[y, x];
            
            int circleCount = 0;
            int triangleCount = 0;
            
            foreach (var sign in signs)
            {
                if (sign.Type == "Circle")
                {
                    circleCount++;
                    // Draw red circle
                    drawCircleOnImage(output, sign.CenterX, sign.CenterY, (int)sign.Radius, 255, 0, 0);
                    // Draw green center cross
                    drawCross(output, sign.CenterX, sign.CenterY, 10, 0, 255, 0);
                }
                else if (sign.Type == "Triangle")
                {
                    triangleCount++;
                    // Draw blue triangle
                    if (sign.Corners != null && sign.Corners.Count >= 3)
                    {
                        drawTriangleOnImage(output, sign.Corners, 0, 0, 255);
                    }
                    // Draw green center cross
                    drawCross(output, sign.CenterX, sign.CenterY, 10, 0, 255, 0);
                }
            }
            
            log.AppendLine($"  Circles drawn: {circleCount} (red)");
            log.AppendLine($"  Triangles drawn: {triangleCount} (blue)");
            
            return output;
        }

        /*
        * drawCircleOnImage: Draw circle outline on RGB image
        */
        private void drawCircleOnImage(byte[,,] image, int cx, int cy, int radius, byte r, byte g, byte b)
        {
            int h = image.GetLength(0);
            int w = image.GetLength(1);
            
            for (int angle = 0; angle < 360; angle++)
            {
                double rad = angle * Math.PI / 180.0;
                
                for (int thickness = -2; thickness <= 2; thickness++)
                {
                    int x = cx + (int)((radius + thickness) * Math.Cos(rad));
                    int y = cy + (int)((radius + thickness) * Math.Sin(rad));
                    
                    if (x >= 0 && x < w && y >= 0 && y < h)
                    {
                        image[y, x, 0] = r;
                        image[y, x, 1] = g;
                        image[y, x, 2] = b;
                    }
                }
            }
        }

        /*
        * drawTriangleOnImage: Draw triangle outline on RGB image
        */
        private void drawTriangleOnImage(byte[,,] image, List<(int x, int y)> corners, byte r, byte g, byte b)
        {
            if (corners.Count < 3) return;
            
            // Draw three edges
            drawLineOnImage(image, corners[0], corners[1], r, g, b);
            drawLineOnImage(image, corners[1], corners[2], r, g, b);
            drawLineOnImage(image, corners[2], corners[0], r, g, b);
        }

        /*
        * drawLineOnImage: Draw line between two points
        */
        private void drawLineOnImage(byte[,,] image, (int x, int y) p1, (int x, int y) p2, byte r, byte g, byte b)
        {
            int h = image.GetLength(0);
            int w = image.GetLength(1);
            
            // Bresenham's line algorithm
            int dx = Math.Abs(p2.x - p1.x);
            int dy = Math.Abs(p2.y - p1.y);
            int sx = p1.x < p2.x ? 1 : -1;
            int sy = p1.y < p2.y ? 1 : -1;
            int err = dx - dy;
            
            int x = p1.x;
            int y = p1.y;
            
            while (true)
            {
                // Draw thick line
                for (int dy2 = -2; dy2 <= 2; dy2++)
                {
                    for (int dx2 = -2; dx2 <= 2; dx2++)
                    {
                        int px = x + dx2;
                        int py = y + dy2;
                        
                        if (px >= 0 && px < w && py >= 0 && py < h)
                        {
                            image[py, px, 0] = r;
                            image[py, px, 1] = g;
                            image[py, px, 2] = b;
                        }
                    }
                }
                
                if (x == p2.x && y == p2.y) break;
                
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        /*
        * drawCross: Draw cross marker at point
        */
        private void drawCross(byte[,,] image, int cx, int cy, int size, byte r, byte g, byte b)
        {
            int h = image.GetLength(0);
            int w = image.GetLength(1);
            
            for (int i = -size; i <= size; i++)
            {
                // Horizontal line
                if (cx + i >= 0 && cx + i < w && cy >= 0 && cy < h)
                {
                    image[cy, cx + i, 0] = r;
                    image[cy, cx + i, 1] = g;
                    image[cy, cx + i, 2] = b;
                }
                
                // Vertical line
                if (cx >= 0 && cx < w && cy + i >= 0 && cy + i < h)
                {
                    image[cy + i, cx, 0] = r;
                    image[cy + i, cx, 1] = g;
                    image[cy + i, cx, 2] = b;
                }
            }
        }

        /*
        * downscaleImage: downscale an image if it exceeds maximum dimensions
        * input:   inputImage      original byte[,] grayscale image
        *          maxDimension    maximum width or height
        * output:                  downscaled byte[,] image (or original if already small enough)
        */
        private byte[,] downscaleImageGrayscale(byte[,] inputImage, int maxDimension)
        {
            int height = inputImage.GetLength(0);
            int width = inputImage.GetLength(1);

            // Check if downscaling is needed
            if (height <= maxDimension && width <= maxDimension)
                return inputImage; // No downscaling needed

            // Calculate new dimensions maintaining aspect ratio
            double scale;
            if (width > height)
                scale = (double)maxDimension / width;
            else
                scale = (double)maxDimension / height;

            int newWidth = (int)(width * scale);
            int newHeight = (int)(height * scale);

            // Create downscaled image using nearest neighbor (fast)
            byte[,] downscaled = new byte[newHeight, newWidth];

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int srcY = (int)(y / scale);
                    int srcX = (int)(x / scale);

                    // Clamp to bounds
                    srcY = Math.Min(srcY, height - 1);
                    srcX = Math.Min(srcX, width - 1);

                    downscaled[y, x] = inputImage[srcY, srcX];
                }
            }

            return downscaled;
        }



        // ====================================================================
        // ============= YOUR FUNCTIONS FOR CHOICE TASKS GO HERE ==============
        // ====================================================================

        /*
        * floodFill:
        * Perform a flood-fill region labeling similar to sequentialRegionLabeling,
        * using 8-neighborhood connectivity.
        * Foreground pixels: 1 (or 255), background: 0
        * Output: grayscale label image (each filled region has unique gray value)
        */
        private byte[,] floodFill(byte[,] inputImage, out int regionCount)
        {
            int h = inputImage.GetLength(0);
            int w = inputImage.GetLength(1);
            byte[,] output = new byte[h, w];

            byte currentLabel = 2;

            // 8-neighborhood offsets
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Found an unfilled foreground pixel
                    if (inputImage[y, x] != 0 && output[y, x] == 0)
                    {
                        // Start a new flood fill
                        Queue<(int, int)> queue = new Queue<(int, int)>();
                        queue.Enqueue((y, x));
                        output[y, x] = currentLabel;

                        while (queue.Count > 0)
                        {
                            var (cy, cx) = queue.Dequeue();

                            for (int i = 0; i < 8; i++)
                            {
                                int ny = cy + dy[i];
                                int nx = cx + dx[i];

                                if (ny >= 0 && ny < h && nx >= 0 && nx < w)
                                {
                                    if (inputImage[ny, nx] != 0 && output[ny, nx] == 0)
                                    {
                                        output[ny, nx] = currentLabel;
                                        queue.Enqueue((ny, nx));
                                    }
                                }
                            }
                        }

                        currentLabel++;

                        if (currentLabel == 255)
                            break;
                    }
                }
            }

            regionCount = currentLabel - 2;
            return output;
        }

        /*
        * findLineCrossings:
        * Given a binary edge map and a list of (r, theta) pairs,
        * find the intersections (crossings) between all line pairs
        * and overlay them as red dots on the input image
        */
        private byte[,,] findLineCrossings(byte[,] edgeImage,
            List<(int rIndex, int thetaIndex, float value)> peaks)
        {
            int h = edgeImage.GetLength(0);
            int w = edgeImage.GetLength(1);

            // Create RGB output image (start from grayscale input)
            byte[,,] output = new byte[h, w, 3];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    output[y, x, 0] = output[y, x, 1] = output[y, x, 2] = edgeImage[y, x];

            double diag = Math.Sqrt(h * h + w * w);
            int rMax = (int)Math.Ceiling(diag);
            int xCenter = w / 2;
            int yCenter = h / 2;

            // Iterate through all unique pairs of lines
            for (int i = 0; i < peaks.Count; i++)
            {
                for (int j = i + 1; j < peaks.Count; j++)
                {
                    var (rIdx1, tIdx1, _) = peaks[i];
                    var (rIdx2, tIdx2, _) = peaks[j];

                    double theta1 = tIdx1 * Math.PI / 180.0;
                    double theta2 = tIdx2 * Math.PI / 180.0;

                    double r1 = rIdx1 - rMax;
                    double r2 = rIdx2 - rMax;

                    double denom = Math.Cos(theta1) * Math.Sin(theta2) - Math.Cos(theta2) * Math.Sin(theta1);
                    if (Math.Abs(denom) < 1e-6)
                        continue;

                    double x = (Math.Sin(theta2) * r1 - Math.Sin(theta1) * r2) / denom;
                    double y = (-Math.Cos(theta2) * r1 + Math.Cos(theta1) * r2) / denom;

                    int px = (int)Math.Round(x + xCenter);
                    int py = (int)Math.Round(y + yCenter);

                    // Draw a small red dot if inside the image
                    if (px >= 1 && px < w - 1 && py >= 1 && py < h - 1)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int yy = py + dy, xx = px + dx;
                                if (yy >= 0 && yy < h && xx >= 0 && xx < w)
                                {
                                    output[yy, xx, 0] = 255; // Red
                                    output[yy, xx, 1] = 0;
                                    output[yy, xx, 2] = 0;
                                }
                            }
                    }
                }
            }

            return output;
        }

        /*
        * applyFindLineCrossings: apply line crossing detection and return RGB result
        * input:   workingImage    single-channel (byte) grayscale image
        * output:                  RGB image with line crossings overlaid
        */
        private byte[,,] applyFindLineCrossings(byte[,] workingImage)
        {
            // Apply Hough transform to detect lines
            float[,] accumulator = houghTransform(workingImage);
            
            // Find peaks in the accumulator
            float peakThreshold = 0.4f;
            var peaks = peakFinding(accumulator, peakThreshold);
            
            // Apply line crossing detection
            byte[,,] crossingsImage = findLineCrossings(workingImage, peaks);
            
            // Calculate number of possible crossings
            int possibleCrossings = peaks.Count * (peaks.Count - 1) / 2;
            
            // Show information about detected crossings
            string crossingInfo = $"Line crossing detection completed\n" +
                                $"Lines detected: {peaks.Count}\n" +
                                $"Possible crossings: {possibleCrossings}\n" +
                                $"Peak threshold used: {peakThreshold:P0} of max value\n\n" +
                                $"Red dots indicate line intersections";
            
            MessageBox.Show(crossingInfo, "Find Line Crossings Results");
            
            return crossingsImage;
        }



    }
}