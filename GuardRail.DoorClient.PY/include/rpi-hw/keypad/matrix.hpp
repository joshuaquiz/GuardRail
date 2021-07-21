/* 
    Title --- keypad/matrix.hpp

    Copyright (C) 2013 Giacomo Trudu - wicker25[at]gmail[dot]com

    This file is part of Rpi-hw.

    Rpi-hw is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation version 3 of the License.

    Rpi-hw is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Rpi-hw. If not, see <http://www.gnu.org/licenses/>.
*/

#ifndef RPI_HW_KEYPAD_MATRIX_HPP
#define RPI_HW_KEYPAD_MATRIX_HPP

#include "../types.hpp"
#include "../exception.hpp"
#include "../utils.hpp"
#include "../time.hpp"
#include "../consts.hpp"
#include "../math.hpp"

#include "../iface/iface_base.hpp"
#include "../iface/output.hpp"
#include "../iface/input.hpp"

#include "keypad_base.hpp"

namespace rpihw {
	namespace keypad {
		/*!
			@class matrix
			@brief Matrix keypad controller.
		*/
		class matrix final : public keypad_base
		{

		public:

			/*!
				@brief Constructor method.
				@param[in] cols Sequence of `uint8_t` containing the column GPIOs.
				@param[in] rows Sequence of `uint8_t` containing the rows GPIOs.
			*/
			matrix(
				std::initializer_list<uint8_t> cols,
				std::initializer_list<uint8_t> rows);

			/*!
				@brief Constructor method.
				@param[in] cols Sequence of `uint8_t` containing the column GPIOs.
				@param[in] rows Sequence of `uint8_t` containing the rows GPIOs.
				@param[in] keymap The keymap vector.
			*/
			matrix(
				std::initializer_list<uint8_t> cols,
				std::initializer_list<uint8_t> rows,
				const std::vector<uint8_t>& keymap);

			matrix() = delete;
			matrix(matrix&) = delete;
			matrix(const matrix&) = delete;
			matrix(matrix&&) = delete;
			matrix& operator=(matrix&) = delete;
			matrix& operator=(const matrix&) = delete;
			matrix& operator=(const matrix&&) = delete;

			//! Destructor method.
			virtual ~matrix();

		protected:

			//! Columns output interface.
			iface::output* m_output_{};

			//! Updates the state of buttons.
			void update() override;
		};
	}
}

// Include inline methods 
#include "matrix-inl.hpp"

#endif /* RPI_HW_KEYPAD_MATRIX_HPP */
