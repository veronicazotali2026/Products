API Template to create a REST service with a list of useful features
-------------------

- Features available
  1. Onion Architecture. Repository interfaces, have been seperated from implementation. This would allow to switch to any DB layer without breaking contracts.
  2. Service Manager. We bring all services, under a single service manager.
  3. API Response types.
  4. CorrelationId and integration with serilog.
  5. HATEOAS
  6. Rate Limit
  7. Caching
  8. Pagination
  9. Sorting
  10. Searching
  11. Filtering
  12. ModelBinders for Posting collections
  13. Validation mechanisms
  14. Command handlers for clean controllers
