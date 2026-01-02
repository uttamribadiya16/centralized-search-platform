import React, { useState } from 'react';
import offerService from '../services/offerService';
import './CreateOfferForm.css';

const CreateOfferForm = ({ sellerId, onOfferCreated }) => {
  const [formData, setFormData] = useState({
    sellerId: sellerId,
    vin: '',
    make: '',
    model: '',
    year: new Date().getFullYear(),
    offerAmount: '',
    condition: 'Good',
    address: ''
  });
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);

  const conditionOptions = ['Excellent', 'Good', 'Fair', 'Poor'];

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    // Only Make, Model, and Year are required
    if (!formData.make.trim()) {
      newErrors.make = 'Make is required';
    }

    if (!formData.model.trim()) {
      newErrors.model = 'Model is required';
    }

    if (!formData.year || formData.year < 1900 || formData.year > new Date().getFullYear() + 1) {
      newErrors.year = 'Please enter a valid year';
    }

    // Optional validations - only check if fields have values
    if (formData.offerAmount && parseFloat(formData.offerAmount) <= 0) {
      newErrors.offerAmount = 'Please enter a valid offer amount';
    }

    if (formData.vin && formData.vin.length !== 17) {
      newErrors.vin = 'VIN must be exactly 17 characters if provided';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      // Prepare data for API - only send fields that have values
      const offerData = {
        sellerId: formData.sellerId,
        make: formData.make,
        model: formData.model,
        year: parseInt(formData.year)
      };

      // Only add optional fields if they have values
      if (formData.vin && formData.vin.trim()) {
        offerData.vin = formData.vin.trim();
      }
      
      if (formData.offerAmount && formData.offerAmount.trim()) {
        offerData.offerAmount = parseFloat(formData.offerAmount);
      }
      
      if (formData.condition && formData.condition !== '') {
        offerData.condition = parseInt(conditionOptions.indexOf(formData.condition));
      }
      
      if (formData.address && formData.address.trim()) {
        offerData.address = formData.address.trim();
      }

      console.log('Submitting offer data:', offerData);

      const result = await offerService.createOffer(offerData);
      console.log('Offer created successfully:', result);
      
      onOfferCreated(result);
      
      // Reset form
      setFormData({
        sellerId: sellerId,
        vin: '',
        make: '',
        model: '',
        year: new Date().getFullYear(),
        offerAmount: '',
        condition: 'Good',
        address: ''
      });
      setErrors({});
      alert('Offer created successfully!');
      
    } catch (error) {
      console.error('Error creating offer:', error);
      setErrors({ submit: error.message || 'Failed to create offer. Please try again.' });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="create-offer-form">
      <h2>Create New Vehicle Offer</h2>
      
      <form onSubmit={handleSubmit}>
        {/* Vehicle Information */}
        <div className="form-section">
          <h3>Vehicle Information</h3>
          
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="vin">VIN (optional)</label>
              <input
                type="text"
                id="vin"
                name="vin"
                value={formData.vin}
                onChange={handleChange}
                maxLength="17"
                placeholder="17-character VIN (optional)"
                disabled={isLoading}
              />
              {errors.vin && <span className="error">{errors.vin}</span>}
            </div>
            
            <div className="form-group">
              <label htmlFor="make">Make *</label>
              <input
                type="text"
                id="make"
                name="make"
                value={formData.make}
                onChange={handleChange}
                placeholder="e.g., Toyota"
                required
                disabled={isLoading}
              />
              {errors.make && <span className="error">{errors.make}</span>}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="model">Model *</label>
              <input
                type="text"
                id="model"
                name="model"
                value={formData.model}
                onChange={handleChange}
                placeholder="e.g., Camry"
                required
                disabled={isLoading}
              />
              {errors.model && <span className="error">{errors.model}</span>}
            </div>
            
            <div className="form-group">
              <label htmlFor="year">Year *</label>
              <input
                type="number"
                id="year"
                name="year"
                value={formData.year}
                onChange={handleChange}
                min="1900"
                max={new Date().getFullYear() + 1}
                required
                disabled={isLoading}
              />
              {errors.year && <span className="error">{errors.year}</span>}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="condition">Condition (optional)</label>
              <select
                id="condition"
                name="condition"
                value={formData.condition}
                onChange={handleChange}
                disabled={isLoading}
              >
                {conditionOptions.map(option => (
                  <option key={option} value={option}>{option}</option>
                ))}
              </select>
            </div>
            
            <div className="form-group">
              <label htmlFor="offerAmount">Offer Amount ($) (optional)</label>
              <input
                type="number"
                id="offerAmount"
                name="offerAmount"
                value={formData.offerAmount}
                onChange={handleChange}
                min="0"
                step="0.01"
                placeholder="Price in USD (optional)"
                disabled={isLoading}
              />
              {errors.offerAmount && <span className="error">{errors.offerAmount}</span>}
            </div>
          </div>
        </div>

        {/* Location */}
        <div className="form-section">
          <h3>Location</h3>
          
          <div className="form-group">
            <label htmlFor="address">Street Address (optional)</label>
            <input
              type="text"
              id="address"
              name="address"
              value={formData.address}
              onChange={handleChange}
              placeholder="Street address (optional)"
              disabled={isLoading}
            />
          </div>
        </div>

        {errors.submit && <div className="error-message">{errors.submit}</div>}

        <div className="form-actions">
          <button
            type="submit"
            className="submit-button"
            disabled={isLoading}
          >
            {isLoading ? 'Creating Offer...' : 'Create Offer'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateOfferForm;