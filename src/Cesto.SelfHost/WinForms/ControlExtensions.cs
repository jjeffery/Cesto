using System.Collections.Generic;
using System.Windows.Forms;

namespace Cesto.WinForms
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Displays a <see cref="Control"/> as a docked child control in a container control, after
        /// clearing out any existing child controls in the container control.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of the container control. Will usually be a subtype of <see cref="ContainerControl"/>,
        ///     <see cref="Panel"/>, or similar.
        /// </typeparam>
        /// <param name="containerControl">The container control.</param>
        /// <param name="userControl">The <see cref="Control"/> to display. This will usually be a user control.</param>
        /// <returns>
        /// Returns <paramref name="containerControl"/>.
        /// </returns>
        /// <remarks>
        /// Any controls that are currently child controls of <paramref name="containerControl"/> are
        /// disposed of.
        /// </remarks>
        public static T Display<T>(this T containerControl, Control userControl) where T : Control
        {
            foreach (Control control in containerControl.Controls) {
                control.Dispose();
            }
            containerControl.Controls.Clear();
            if (userControl != null) {
                userControl.Dock = DockStyle.Fill;
                userControl.Visible = true;
                containerControl.Controls.Add(userControl);
            }

            var cc = containerControl as IContainerControl;
            if (cc != null)
            {
                cc.ActiveControl = userControl;
            }

            return containerControl;
        }

        /// <summary>
        /// Displays a <see cref="UserControl"/> as a docked child control in a container control, after
        /// hiding any existing controls in the container control. The existing controls are restored after
        /// the child control becomes invisible or is disposed.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of the container control. Will usually be a subtype of <see cref="ContainerControl"/>,
        ///     <see cref="Panel"/>, or similar.
        /// </typeparam>
        /// <param name="containerControl">The container control.</param>
        /// <param name="userControl">The <see cref="Control"/> to display. This will usually be a user control.</param>
        /// <returns>
        /// Returns <paramref name="containerControl"/>.
        /// </returns>
        /// <remarks>
        /// Any controls that are currently child controls of <paramref name="containerControl"/> are
        /// made invisible for the duration that <paramref name="userControl"/> is visible, and restored once 
        /// <paramref name="userControl"/> is disposed of or becomes invisible.
        /// </remarks>
        public static T DisplayModal<T>(this T containerControl, Control userControl) where T : Control
        {
            var cc = containerControl as IContainerControl;
            var visibleControls = new List<Control>();
            var activeControl = cc == null ? null : cc.ActiveControl;

            foreach (Control control in containerControl.Controls) {
                if (control.Visible) {
                    visibleControls.Add(control);
                    control.Visible = false;
                }
            }
            if (userControl != null) {
                userControl.Dock = DockStyle.Fill;
                userControl.Visible = true;
                containerControl.Controls.Add(userControl);

                userControl.Disposed += (sender, e) => {
                    containerControl.Controls.Remove(userControl);
                    foreach (var visibleControl in visibleControls) {
                        visibleControl.Visible = true;
                    }
                    if (cc != null && activeControl != null) {
                        cc.ActiveControl = activeControl;
                    }
                };

                userControl.VisibleChanged += (sender, e) => {
                    var control = (Control)sender;
                    if (!control.Visible) {
                        containerControl.Controls.Remove(userControl);
                        foreach (var visibleControl in visibleControls) {
                            visibleControl.Visible = true;
                        }
                        control.Dispose();
                        if (cc != null && activeControl != null) {
                            cc.ActiveControl = activeControl;
                        }
                    }
                };
            }

            return containerControl;
        }
    }
}
