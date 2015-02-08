using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace _2_1_galleriet
{
    public partial class Default : System.Web.UI.Page
    {
        private void renderErrorMessage(string msg)
        {
            CustomValidator customValidator = new CustomValidator();
            customValidator.IsValid = false;
            customValidator.ErrorMessage = msg;
            this.Page.Validators.Add(customValidator);
        }

        private string getPageName()
        {
            return Path.GetFileName(Page.AppRelativeVirtualPath);
        }

        private void renderImage(string imageSrc)
        {
            ImageContainer.Controls.Add(new HtmlImage() {
                Src=imageSrc
            });

            ImageContainer.Visible = true;
        }

        private void renderThumbnails()
        {
            string[] imageList, thumbNailList;
            Gallery galleryObj = new Gallery();
            DataTable imageTable;
            DataRow imageTableRow;
            DataSet imageDataSet;
            string imageGET;

            // Get image name from get request
            imageGET = Request.QueryString["img"];

            // Get previously uploaded images
            thumbNailList = galleryObj.GetImageNames(Gallery.ImageType.Thumbnail);
            imageList = galleryObj.GetImageNames(Gallery.ImageType.LargeImage);

            // Render thumbnails only if there are any
            if (thumbNailList.Length > 0)
            {
                // Build up a datasource containing thumbnaillist and imagelist
                imageTable = new DataTable("imageTable");
                imageTable.Columns.Add(new DataColumn("imagePath", typeof(string)));
                imageTable.Columns.Add(new DataColumn("thumbNailPath", typeof(string)));
                imageTable.Columns.Add(new DataColumn("cssClass", typeof(string)));

                // Loop through thumbnails, fill datasource with data.
                for (int i = 0; i < thumbNailList.Length; i++)
                {
                    // New rable row
                    imageTableRow = imageTable.NewRow();

                    // New cells with data
                    imageTableRow[0] = String.Format("{0}?img={1}", getPageName(), (imageList.Length == thumbNailList.Length ? imageList[i] : ""));
                    imageTableRow[1] = String.Format("{0}/{1}", Gallery.THUMBNAIL_PATH, thumbNailList[i]);

                    // Render css active class if this image should be active, compare GET request to image filename
                    if (imageGET != null && imageList.Length == thumbNailList.Length && imageGET == imageList[i])
                    {
                        imageTableRow[2] = "active";
                    }

                    // Add row to image data table
                    imageTable.Rows.Add(imageTableRow);
                }

                // Create dataset from table with data
                imageDataSet = new DataSet("imageDataSet");
                imageDataSet.Tables.Add(imageTable);

                // Add image dataset as souce and bind it to Repeater
                ThumbnailRepeater.DataSource = imageDataSet;
                ThumbnailRepeater.DataBind();

                // Show Thumbnail container
                ThumbnailContainer.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string image;
            Gallery gallery = new Gallery();

            // Get image name from get request
            image = Request.QueryString["img"];

            // Render image only if it exists
            if (image != null && gallery.ImageExists(image))
            {
                renderImage(String.Format("{0}/{1}", Gallery.IMAGE_PATH, image));
            }

            if (!Page.IsPostBack)
            {
                // Render thumbnails must me called separately, to include uploaded image.
                renderThumbnails();
            }
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if(IsValid)
            {
                Gallery galleryObj;
                string fileName;

                // If no file was uploaded
                if(!FileUpload.HasFile)
                {
                    renderErrorMessage("Var god välj en fil som ska laddas upp.");
                    return;
                }

                // Create gallery object
                galleryObj = new Gallery();
                
                // Try to save image
                try
                {
                    fileName = galleryObj.SaveImage(
                                        FileUpload.PostedFile.InputStream,
                                        Path.GetFileName(FileUpload.PostedFile.FileName)
                                    );

                    InfoPanel.Visible = true;
                    InfoPanel.CssClass = "success-message";
                    InfoPanelLiteral.Text = String.Format("Bilden '{0}' har laddats upp.", fileName);

                    //Response.Redirect(String.Format("{0}?img={1}", getPageName(), fileName));

                    //renderImage(String.Format("{0}/{1}", Gallery.IMAGE_PATH, fileName));
                }
                catch (Exception error)
                {
                    renderErrorMessage(error.Message);
                }
            }

            renderThumbnails();
            
        }
    }
}