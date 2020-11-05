/*
 * .NET bindings for libheif.
 * Copyright (c) 2020 Nicholas Hayes
 *
 * Portions Copyright (c) 2017 struktur AG, Dirk Farin <farin@struktur.de>
 *
 * This file is part of libheif-sharp.
 *
 * libheif-sharp is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 *
 * libheif-sharp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with libheif-sharp.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace LibHeifSharp.Interop
{
    internal enum heif_suberror_code
    {
        // no further information available
        Unspecified = 0,

        // --- Invalid_input ---

        // End of data reached unexpectedly.
        End_of_data = 100,

        // Size of box (defined in header) is wrong
        Invalid_box_size = 101,

        // Mandatory 'ftyp' box is missing
        No_ftyp_box = 102,

        No_idat_box = 103,

        No_meta_box = 104,

        No_hdlr_box = 105,

        No_hvcC_box = 106,

        No_pitm_box = 107,

        No_ipco_box = 108,

        No_ipma_box = 109,

        No_iloc_box = 110,

        No_iinf_box = 111,

        No_iprp_box = 112,

        No_iref_box = 113,

        No_pict_handler = 114,

        // An item property referenced in the 'ipma' box is not existing in the 'ipco' container.
        Ipma_box_references_nonexisting_property = 115,

        // No properties have been assigned to an item.
        No_properties_assigned_to_item = 116,

        // Image has no (compressed) data
        No_item_data = 117,

        // Invalid specification of image grid (tiled image)
        Invalid_grid_data = 118,

        // Tile-images in a grid image are missing
        Missing_grid_images = 119,

        Invalid_clean_aperture = 120,

        // Invalid specification of overlay image
        Invalid_overlay_data = 121,

        // Overlay image completely outside of visible canvas area
        Overlay_image_outside_of_canvas = 122,

        Auxiliary_image_type_unspecified = 123,

        No_or_invalid_primary_item = 124,

        No_infe_box = 125,

        Unknown_color_profile_type = 126,

        Wrong_tile_image_chroma_format = 127,

        Invalid_fractional_number = 128,

        Invalid_image_size = 129,

        Invalid_pixi_box = 130,

        No_av1C_box = 131,

        // --- Memory_allocation_error ---

        // A security limit preventing unreasonable memory allocations was exceeded by the input file.
        // Please check whether the file is valid. If it is, contact us so that we could increase the
        // security limits further.
        Security_limit_exceeded = 1000,

        // --- Usage_error ---

        // An item ID was used that is not present in the file.
        Nonexisting_item_referenced = 2000, // also used for Invalid_input

        // An API argument was given a NULL pointer, which is not allowed for that function.
        Null_pointer_argument = 2001,

        // Image channel referenced that does not exist in the image
        Nonexisting_image_channel_referenced = 2002,

        // The version of the passed plugin is not supported.
        Unsupported_plugin_version = 2003,

        // The version of the passed writer is not supported.
        Unsupported_writer_version = 2004,

        // The given (encoder) parameter name does not exist.
        Unsupported_parameter = 2005,

        // The value for the given parameter is not in the valid range.
        Invalid_parameter_value = 2006,

        // --- Unsupported_feature ---

        // Image was coded with an unsupported compression method.
        Unsupported_codec = 3000,

        // Image is specified in an unknown way, e.g. as tiled grid image (which is supported)
        Unsupported_image_type = 3001,

        Unsupported_data_version = 3002,

        // The conversion of the source image to the requested chroma / colorspace is not supported.
        Unsupported_color_conversion = 3003,

        Unsupported_item_construction_method = 3004,

        // --- Encoder_plugin_error ---

        Unsupported_bit_depth = 4000,

        // --- Encoding_error ---

        Cannot_write_output_data = 5000,
    }
}
