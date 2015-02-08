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
            thumbNailList = galleryObj.GetImageNames(Gallery.ImageType.Thumbnail).ToArray();
            imageList = galleryObj.GetImageNames(Gallery.ImageType.LargeImage).ToArray();

            // Build up a datasource containing thumbnaillist and imagelist
            imageTable = new DataTable("imageTable");
            imageTable.Columns.Add(new DataColumn("imagePath", typeof(string)));
            imageTable.Columns.Add(new DataColumn("thumbNailPath", typeof(string)));
            imageTable.Columns.Add(new DataColumn("cssClass", typeof(string)));

            for(int i = 0; i < imageList.ToArray().Length; i++)
            {
                imageTableRow = imageTable.NewRow();

                imageTableRow[0] = String.Format("{0}?img={1}", getPageName(), imageList[i]);
                imageTableRow[1] = String.Format("{0}/{1}", Gallery.THUMBNAIL_PATH, thumbNailList[i]);

                // Render css active class if this image should be active, compare GET request to image filename
                if (imageGET != null && imageGET == imageList[i])
                {
                    imageTableRow[2] = "active";
                }

                imageTable.Rows.Add(imageTableRow);
            }

            imageDataSet = new DataSet("imageDataSet");
            imageDataSet.Tables.Add(imageTable);

            ThumbnailRepeater.DataSource = imageDataSet;
            ThumbnailRepeater.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                string image;
                Gallery gallery = new Gallery();

                // Render thumbnails
                renderThumbnails();

                // Get image name from get request
                image = Request.QueryString["img"];

                // Render image only if it exists
                if (image != null && gallery.ImageExists(image))
                {
                    renderImage(String.Format("{0}/{1}", Gallery.IMAGE_PATH, image));
                }
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
                    InfoPanelLiteral.Text = String.Format("Bilden {0} har laddats upp.", fileName);

                    //Response.Redirect(String.Format("{0}?img{1}", getPageName(), fileName));
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